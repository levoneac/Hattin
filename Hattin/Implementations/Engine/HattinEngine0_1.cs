using System.Collections;
using System.Diagnostics;
using Hattin.Extensions.Move;
using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.SideToMove;
using Hattin.Interfaces;
using Hattin.Types;
using Hattin.Utils;

namespace Hattin.Implementations.Engine
{
    public class HattinEngine0_1 : IEngine
    {
        public BoardState Board { get; init; }
        public IMoveGenerator MoveGenerator { get; init; }
        public IMoveConstraintBuilder MoveConstraintBuilder { get; init; }
        public IPositionEvaluator PositionEvaluator { get; init; }
        private TranspositionTable<Transposition> TranspositionTable;
        public HattinEngine0_1(BoardState board, IMoveGenerator moveGenerator, IMoveConstraintBuilder moveConstraintBuilder, IPositionEvaluator positionEvaluator)
        {
            Board = board;
            MoveGenerator = moveGenerator;
            MoveConstraintBuilder = moveConstraintBuilder;
            PositionEvaluator = positionEvaluator;
            TranspositionTable = new TranspositionTable<Transposition>(100_000);
        }

        //Gets the constraints based on the current boardstate
        private Func<List<GeneratedMove>, List<GeneratedMove>>? GetConstraintFuncs(List<BoardSquare>? checkingSources = null)
        {
            MoveConstraintBuilder.Reset();
            if (checkingSources is not null && checkingSources.Count > 0)
            {
                MoveConstraintBuilder.SetStopCheck(checkingSources);
            }
            MoveConstraintBuilder.SetPinRestriction();
            return MoveConstraintBuilder.GetConstraintFunction();
        }

        //Change to inplace sort later
        private List<GeneratedMove> OrderMoves(List<GeneratedMove> moves)
        {
            List<(int, GeneratedMove)> scoreAndMove = [];
            foreach (GeneratedMove move in moves)
            {
                int score = 0;
                if (move.IsCheck) { score += 10; }
                if (move.IsCapture) { score += 5; }
                if (move.IsPromotion) { score += 5; }

                AttackInformation destinationAttackInfo = Board.PieceProperties.GetAttackCountOnSquare(move.DestSquare);
                int sumAttacks = destinationAttackInfo.AttackTotals.White - destinationAttackInfo.AttackTotals.Black;

                //Need one more attacker than defender to win the exchange
                if (sumAttacks > 0 && Board.SideToMove == SideToMove.White) { score += 8; }
                else if (sumAttacks < 0 && Board.SideToMove == SideToMove.Black) { score += 8; }

                foreach (BoardSquare attackedFromSquare in Board.PieceProperties.GetAttackSource(move.DestSquare))
                {
                    NormalPieceValue capturingPieceValue = Board.PieceProperties.GetPieceOnSquare(attackedFromSquare).ToValue();
                    if (capturingPieceValue < move.Piece.ToValue() || capturingPieceValue == NormalPieceValue.King)
                    {
                        score -= 3;
                    }
                }
                scoreAndMove.Add((score, move));
            }
            return scoreAndMove.OrderByDescending(key => key.Item1).Select(i => i.Item2).ToList();
        }

        private MoveEvaluation AlphaBetaSearch(GeneratedMove move, BoardState currentBoard, int depth, int absoluteDepth, float alpha, float beta, SideToMove player)
        {

            MoveEvaluation bestMove = new MoveEvaluation(player);
            MoveEvaluation curEval;
            GeneratedMove priorityMove = null;
            GeneratedMove curMove;
            List<GeneratedMove> possibleMoves = new List<GeneratedMove>();
            TranspositionEntryType transpositionEntryType = TranspositionEntryType.FullySearched;
            bool tryFromTableFirst = false;

            int positionHash = Board.PositionHash.CurrentPositionHash;
            //Check if the current position has occured before in the current game
            if (move.Piece != NormalPiece.Empty)
            {
                if (Board.RepetitionTable.ProbePosition(positionHash))
                {
                    //Needed because it always gets popped afterwards
                    Board.RepetitionTable.PushPosition(positionHash);

                    //Returns an almost equal score because (3-fold) repetition is a draw
                    //Added a small penalty to encourage the engine to play other drawn moves before going for repetiotion
                    bestMove.SetToNewMove(move, Board.SideToMove == SideToMove.White ? 10 : -10);
                    return bestMove;
                }
                Board.RepetitionTable.PushPosition(positionHash);
            }

            //Check if the current position has occured in a different branch of the search tree
            if (TranspositionTable.TryGetValue(positionHash, out Transposition preCalculated))
            {
                //If the found position was searched longer than this one will be, return the move
                if (preCalculated.Depth >= depth && preCalculated.Type == TranspositionEntryType.FullySearched)
                {
                    bestMove.SetToNewMove(preCalculated.Move, preCalculated.Evaluation);
                    return bestMove;
                }
                //Else try the found move first for possible fast pruning
                else
                {
                    priorityMove = preCalculated.Move;
                }
            }

            //Evaluate the position if the depth limit has been reached
            if (depth <= 0)
            {
                return new MoveEvaluation(move, PositionEvaluator.EvaluateCurrentPosition(currentBoard));
            }

            //Try to order the moves in such a way that we prune as many branches as possible
            possibleMoves.AddRange(GetPossibleMoves());
            possibleMoves = OrderMoves(possibleMoves);
            if (priorityMove is not null)
            {
                //Due to hash collisions you need to check if the move is actually legal
                int priorityMoveIndex = possibleMoves.IndexOf(priorityMove);
                if (priorityMoveIndex != -1)
                {
                    possibleMoves.RemoveAt(priorityMoveIndex);
                    possibleMoves.Insert(0, priorityMove);
                }
            }

            //If not moves are possible, it means the position is either mate or stalemate
            if (possibleMoves.Count == 0)
            {
                return ResolveNoMoves(player);
            }

            //Search for white
            if (player == SideToMove.White)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    curMove = possibleMoves[i];
                    Board.MovePiece(curMove, true);
                    curEval = AlphaBetaSearch(curMove, currentBoard, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, beta, player.ToOppositeColor());
                    Board.RepetitionTable.PopPosition();
                    Board.UndoLastMove(true);

                    if (curEval.Evaluation > bestMove.Evaluation)
                    {
                        bestMove.SetToNewMove(curMove, curEval.Evaluation);
                        if (absoluteDepth == 0) { Console.WriteLine($"info score cp {bestMove.Evaluation} pv {bestMove.Move.ToAlgebra()}"); }
                    }

                    if (bestMove.Evaluation > alpha)
                    {
                        alpha = bestMove.Evaluation;
                    }

                    if (alpha >= beta)
                    {
                        transpositionEntryType = TranspositionEntryType.Pruned;
                        break;
                    }

                    if (tryFromTableFirst)
                    {
                        tryFromTableFirst = false;
                        possibleMoves.AddRange(GetPossibleMoves());
                    }
                }
                TranspositionTable[positionHash] = new Transposition(bestMove, depth, transpositionEntryType);


                return bestMove;
            }

            //Search for black
            if (player == SideToMove.Black)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    curMove = possibleMoves[i];
                    Board.MovePiece(curMove, true);
                    curEval = AlphaBetaSearch(curMove, currentBoard, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, beta, player.ToOppositeColor());
                    Board.RepetitionTable.PopPosition();
                    Board.UndoLastMove(true);

                    if (curEval.Evaluation < bestMove.Evaluation)
                    {
                        bestMove.SetToNewMove(curMove, curEval.Evaluation);
                        if (absoluteDepth == 0) { Console.WriteLine($"info score cp {bestMove.Evaluation} pv {bestMove.Move.ToAlgebra()}"); }
                    }

                    if (bestMove.Evaluation < beta)
                    {
                        beta = bestMove.Evaluation;
                    }

                    if (beta <= alpha)
                    {
                        transpositionEntryType = TranspositionEntryType.Pruned;
                        break;
                    }

                    if (tryFromTableFirst)
                    {
                        tryFromTableFirst = false;
                        possibleMoves.AddRange(GetPossibleMoves());
                    }
                }
                TranspositionTable[positionHash] = new Transposition(bestMove, depth, transpositionEntryType);

                return bestMove;
            }
            return new MoveEvaluation(bestMove.Move, (int)GameResult.Draw);

        }

        private MoveEvaluation ResolveNoMoves(SideToMove player)
        {
            GeneratedMove noMove = new GeneratedMove();
            //Mate
            if (Board.IsCheck)
            {
                Board.GameResult = player == SideToMove.White ? GameResult.BlackWin : GameResult.WhiteWin;
                return new MoveEvaluation(noMove, 100000000 * (int)Board.GameResult);
            }
            //Stalemate
            else
            {
                Board.GameResult = GameResult.Draw;
                return new MoveEvaluation(noMove, 0);
            }
        }

        private static int ExtendSearch(GeneratedMove move, int depth)
        {
            if (depth == 0)
            {
                int extension = 0;
                if (move.IsCheck || move.IsCapture)
                {
                    extension = 1;
                }
                return extension;
            }
            return 0;

        }

        public List<GeneratedMove> GetPossibleMoves()
        {
            Board.PieceProperties.UpdateAllAttackSquares(MoveGenerator.GenerateAllAttackedSquares());
            List<BoardSquare> checkingSources = Board.PieceProperties.GetCheckSource(Board.SideToMove);
            if (checkingSources.Count > 0)
            {
                Board.IsCheck = true;
            }
            else
            {
                Board.IsCheck = false;
            }
            return MoveGenerator.GenerateAllLegalMoves(GetConstraintFuncs(checkingSources));
        }

        //Mostly for testingpurposes still
        public GeneratedMove GetNextMove()
        {
            List<GeneratedMove> generatedMoves = GetPossibleMoves();
            GeneratedMove chosenMove;
            if (generatedMoves.Count > 0)
            {
                //chosenMove = generatedMoves?[new Random().Next(0, generatedMoves.Count - 1)] ?? new GeneratedMove();
                //TranspositionTable.Clear();
                MoveEvaluation bestMove = AlphaBetaSearch(new GeneratedMove(), Board, 5, 0, float.MinValue, float.MaxValue, Board.SideToMove);
                Console.WriteLine($"info score cp {bestMove.Evaluation} pv {bestMove.Move.ToAlgebra()}");
                chosenMove = bestMove.Move;
            }
            else
            {
                chosenMove = new GeneratedMove();
            }
            if (chosenMove.Piece == NormalPiece.Empty)
            {
                Board.PrintBoard(SideToMove.White);
                throw new Exception("$GAME OVER");
            }
            return chosenMove;
        }

        //Testing move gen
        public void PlayChosenMove(GeneratedMove chosenMove)
        {
            Board.MovePiece(chosenMove);
            if (chosenMove.IsCheck)
            {
                Board.IsCheck = true;
            }
            else
            {
                Board.IsCheck = false;
            }
        }

        //Autoplay test
        public void PlayUntillPly(object? plyCount)
        {
            while (Board.PlyCounter <= (int)plyCount)
            {
                PlayChosenMove(GetNextMove());
                Board.PrintBoard(SideToMove.White);
                Thread.Sleep(3000);
            }
        }

        //Dummy function for testing right now
        public void AnalyzeCurrent(AnalyzedPosition analyzedPosition)
        {
            GeneratedMove chosenMove = GetNextMove();
            analyzedPosition.BestMove = new MoveEvaluation(chosenMove, 1.0f);
            analyzedPosition.IsDone = true;
            return;
            //while (Board.PlyCounter <= 1000 && !analyzedPosition.StopToken.IsCancellationRequested)
            //{
            //    PlayChosenMove(GetNextMove());
            //    Board.PrintBoard(SideToMove.White);
            //    Thread.Sleep(3000);
            //}
        }

        //Wrapper for void AnalyzeCurrent(AnalyzedPosition analyzedPosition), needed for Thread instance
        public void AnalyzeCurrent(object? options)
        {
            if (options is null || options.GetType() != typeof(AnalyzedPosition))
            {
                throw new ArgumentNullException(nameof(options), $"This function is just a wrapper for the main AnalyzeCurrent and requires an AnalyzedPosition instance");
            }
            AnalyzeCurrent((AnalyzedPosition)options);
        }
    }
}