using System.Collections;
using System.Diagnostics;
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


        //BUG 1: if the transptable is small, the engine sometimes cant generate moves even if possible moves exist
        private MoveEvaluation AlphaBetaSearch(GeneratedMove move, BoardState currentBoard, int depth, int absoluteDepth, int maxDepth, float alpha, float beta, SideToMove player)
        {

            MoveEvaluation bestMove = new MoveEvaluation(player);
            MoveEvaluation curEval;
            GeneratedMove curMove;
            List<GeneratedMove> possibleMoves = new List<GeneratedMove>();
            bool tryFromTableFirst = false;

            int positionHash = Board.PositionHash.CurrentPositionHash;
            if (TranspositionTable.TryGetValue(positionHash, out Transposition preCalculated))
            {
                //If the found position was searched longer than this one will be, return the move
                if (preCalculated.Depth > depth && preCalculated.Type == TranspositionEntryType.FullySearched)
                {
                    bestMove.SetToNewMove(preCalculated.Move, preCalculated.Evaluation);
                    return bestMove;
                }

                //Else try the found move first for possible fast pruning
                if (Board.IsValid(preCalculated.Move))
                {
                    possibleMoves.Add(preCalculated.Move);
                    tryFromTableFirst = true;
                }

            }
            if (depth <= 0 || absoluteDepth >= maxDepth)
            {
                return new MoveEvaluation(move, PositionEvaluator.EvaluateCurrentPosition(currentBoard));
            }


            TranspositionEntryType transpositionEntryType = TranspositionEntryType.FullySearched;
            if (!tryFromTableFirst)
            {
                possibleMoves.AddRange(GetPossibleMoves());
            }

            if (possibleMoves.Count == 0)
            {
                return ResolveNoMoves(player);
            }
            if (player == SideToMove.White)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    curMove = possibleMoves[i];
                    Board.MovePiece(curMove);
                    curEval = AlphaBetaSearch(curMove, currentBoard, depth - 1 + ExtendSearch(curMove), absoluteDepth + 1, maxDepth, alpha, beta, player.ToOppositeColor());

                    if (curEval.Evaluation > bestMove.Evaluation)
                    {
                        bestMove.SetToNewMove(curMove, curEval.Evaluation);
                    }

                    Board.UndoLastMove();
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

            if (player == SideToMove.Black)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    curMove = possibleMoves[i];
                    Board.MovePiece(curMove);
                    curEval = AlphaBetaSearch(curMove, currentBoard, depth - 1 + ExtendSearch(curMove), absoluteDepth + 1, maxDepth, alpha, beta, player.ToOppositeColor());
                    if (curEval.Evaluation < bestMove.Evaluation)
                    {
                        bestMove.SetToNewMove(curMove, curEval.Evaluation);
                    }

                    Board.UndoLastMove();
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

        private int ExtendSearch(GeneratedMove move)
        {
            int extension = 0;
            if (move.IsCheck || move.IsCapture)
            {
                extension = 1;
            }
            return extension;
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
                MoveEvaluation bestMove = AlphaBetaSearch(new GeneratedMove(), Board, 3, 0, 8, float.MinValue, float.MaxValue, Board.SideToMove);
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
            if (chosenMove.IsPromotion)
            {
                NormalPiece piece = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteQueen : NormalPiece.BlackQueen;
                chosenMove.PromoteTo = piece;
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