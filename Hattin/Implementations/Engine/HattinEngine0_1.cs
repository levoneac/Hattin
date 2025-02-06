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
        private Stack<GeneratedMove> PV;
        private static long NodeCounter;
        private List<List<GeneratedMove>> KillerMoves;
        public HattinEngine0_1(BoardState board, IMoveGenerator moveGenerator, IMoveConstraintBuilder moveConstraintBuilder, IPositionEvaluator positionEvaluator)
        {
            Board = board;
            MoveGenerator = moveGenerator;
            MoveConstraintBuilder = moveConstraintBuilder;
            PositionEvaluator = positionEvaluator;
            TranspositionTable = new TranspositionTable<Transposition>(100_000);
            PV = new Stack<GeneratedMove>();
            NodeCounter = 0;
            KillerMoves = new List<List<GeneratedMove>>();
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

        private List<GeneratedMove> FilterAndOrderTacticalMoves(List<GeneratedMove> moves)
        {
            List<(int, GeneratedMove)> scoreAndMove = [];
            foreach (GeneratedMove move in moves)
            {
                NormalPiece[] promotionClass = NormalPieceClassifications.GetPiececlassFromPiece(move.PromoteTo);
                if (!move.IsCapture || promotionClass == NormalPieceClassifications.Queens || promotionClass == NormalPieceClassifications.Knights) { continue; }
                int score = 0;
                NormalPieceValue pieceValue = move.Piece.ToValue();
                if (move.IsCheck) { score += 15_000; }
                if (move.IsPromotion) { score += 5_000; }

                //Increase score if the piece you are capturing is lower valued
                score += Board.PieceProperties.GetPieceOnSquare(move.DestSquare).ToValue() - pieceValue + 10_000;

                AttackInformation destinationAttackInfo = Board.PieceProperties.GetAttackCountOnSquare(move.DestSquare);
                int sumAttacks = destinationAttackInfo.AttackTotals.White - destinationAttackInfo.AttackTotals.Black;

                //Increase score if you can win the exchange
                if (sumAttacks > 0 && Board.SideToMove == SideToMove.White) { score += 8_000; }
                else if (sumAttacks < 0 && Board.SideToMove == SideToMove.Black) { score += 8_000; }

                //Reduce the score if a lower valued piece can recapture
                foreach (BoardSquare attackedFromSquare in Board.PieceProperties.GetAttackSource(move.DestSquare))
                {
                    NormalPieceValue capturingPieceValue = Board.PieceProperties.GetPieceOnSquare(attackedFromSquare).ToValue();
                    if (capturingPieceValue < pieceValue || capturingPieceValue == NormalPieceValue.King)
                    {
                        score -= pieceValue - capturingPieceValue;
                    }
                }
                scoreAndMove.Add((score, move));
            }
            return scoreAndMove.OrderByDescending(key => key.Item1).Select(i => i.Item2).ToList();
        }

        //Change to inplace sort later
        private List<GeneratedMove> OrderMoves(List<GeneratedMove> moves)
        {
            List<(int, GeneratedMove)> scoreAndMove = [];
            foreach (GeneratedMove move in moves)
            {
                int score = 0;
                NormalPieceValue pieceValue = move.Piece.ToValue();
                if (move.IsCheck) { score += 9_000; }
                if (move.IsPromotion) { score += 5_000; }
                if (move.IsCapture)
                {
                    //Increase score if the piece you are capturing is lower valued
                    score += Board.PieceProperties.GetPieceOnSquare(move.DestSquare).ToValue() - pieceValue + 10_000;
                }

                AttackInformation destinationAttackInfo = Board.PieceProperties.GetAttackCountOnSquare(move.DestSquare);
                int sumAttacks = destinationAttackInfo.AttackTotals.White - destinationAttackInfo.AttackTotals.Black;

                //Increase score if you can win the exchange
                if (Board.SideToMove == SideToMove.White && destinationAttackInfo.AttackTotals.Black > 0 && sumAttacks > 0) { score += 8_000; }
                else if (Board.SideToMove == SideToMove.Black && destinationAttackInfo.AttackTotals.White > 0 && sumAttacks < 0) { score += 8_000; }

                //Reduce the score if a lower valued piece can recapture
                foreach (BoardSquare attackedFromSquare in Board.PieceProperties.GetAttackSource(move.FromSquare))
                {
                    NormalPiece capturingPiece = Board.PieceProperties.GetPieceOnSquare(attackedFromSquare);
                    if (capturingPiece.ToColor() != move.Piece.ToColor())
                    {
                        NormalPieceValue capturingPieceValue = capturingPiece.ToValue();
                        if (capturingPieceValue < pieceValue || capturingPieceValue == NormalPieceValue.King)
                        {
                            score -= pieceValue - capturingPieceValue;
                        }
                    }

                }
                scoreAndMove.Add((score, move));
            }
            return scoreAndMove.OrderByDescending(key => key.Item1).Select(i => i.Item2).ToList();
        }

        private MoveEvaluation AlphaBetaSearch(GeneratedMove move, int depth, int absoluteDepth, int alpha, int beta, SideToMove player, List<GeneratedMove> pVStack)
        {
            NodeCounter++;
            MoveEvaluation bestMove = new MoveEvaluation(player);
            MoveEvaluation curEval;
            GeneratedMove? priorityMove = null;
            GeneratedMove curMove;
            List<GeneratedMove> possibleMoves = new List<GeneratedMove>();
            TranspositionEntryType transpositionEntryType = TranspositionEntryType.FullySearched;

            //Initialize killer moves for new depths
            if (KillerMoves.Count == absoluteDepth)
            {
                //Add two moves, in case the first move is just an evasion move for a specific treath
                KillerMoves.Add(new List<GeneratedMove>(2));
            }

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
                    bestMove.SetToNewMove(move, Board.SideToMove == SideToMove.White ? 10 : -10, [move]);
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
                    bestMove.SetToNewMove(preCalculated.Move, preCalculated.Evaluation, [preCalculated.Move]);
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
                return new MoveEvaluation(move, (int)player * QuiessenceSearch(int.MinValue, int.MaxValue, player));
            }

            //Try to order the moves in such a way that we prune as many branches as possible
            possibleMoves.AddRange(GetPossibleMoves());
            possibleMoves = OrderMoves(possibleMoves);

            int priorityPlacement = 0;
            //Try the PV first
            if (pVStack.Count > 0)
            {
                int pvStackArrayPointer = depth - 2;
                if (pvStackArrayPointer < pVStack.Count && pvStackArrayPointer >= 0)
                {
                    GeneratedMove pVMove = pVStack[pvStackArrayPointer];

                    if (!possibleMoves.Remove(pVMove)) { pVStack.Clear(); }
                    else
                    {
                        possibleMoves.Insert(0, pVMove);
                        priorityPlacement++;
                    }
                }
                else { pVStack.Clear(); }
            }

            //If there was a hit in the transposition table, then move it first if its not already the same move as in PV
            if (priorityMove is not null)
            {
                //Due to hash collisions you need to check if the move is actually legal
                int priorityMoveIndex = possibleMoves.IndexOf(priorityMove);
                if (priorityMoveIndex > priorityPlacement)
                {
                    possibleMoves.RemoveAt(priorityMoveIndex);
                    possibleMoves.Insert(priorityPlacement, priorityMove);
                    priorityPlacement++;
                }
            }

            foreach (GeneratedMove killerMove in KillerMoves[absoluteDepth])
            {
                int killerIndex = possibleMoves.IndexOf(killerMove);
                if (killerIndex > priorityPlacement)
                {
                    possibleMoves.RemoveAt(killerIndex);
                    possibleMoves.Insert(priorityPlacement, killerMove);
                    priorityPlacement++;
                }
            }

            //If not moves are possible, it means the position is either mate or stalemate
            if (possibleMoves.Count == 0) { return ResolveNoMoves(player); }

            //Search for white
            if (player == SideToMove.White)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    curMove = possibleMoves[i];
                    Board.MovePiece(curMove, true);
                    //The first move gets a proper search
                    if (i == 0)
                    {
                        curEval = AlphaBetaSearch(curMove, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, beta, player.ToOppositeColor(), pVStack);
                    }
                    else
                    {//moves afterward gets a tiny windows to prune faster (Asumes that the first move that was searched was the best)
                        curEval = AlphaBetaSearch(curMove, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, alpha + 1, player.ToOppositeColor(), pVStack);
                        //If a better move is found then we need to do a proper search, hopefully not every time
                        if (alpha > curEval.Evaluation && curEval.Evaluation < beta)
                        {
                            Board.RepetitionTable.PopPosition();
                            curEval = AlphaBetaSearch(curMove, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, beta, player.ToOppositeColor(), pVStack);
                        }
                    }
                    Board.RepetitionTable.PopPosition();
                    Board.UndoLastMove(true);

                    if (curEval.Evaluation > bestMove.Evaluation)
                    {
                        bestMove.SetToNewMove(curMove, curEval.Evaluation, curEval.PV);
                        //Console.WriteLine($"info score cp {bestMove.Evaluation} pv {bestMove.Move.ToAlgebra()} depth {absoluteDepth}");
                    }

                    if (bestMove.Evaluation > alpha)
                    {
                        alpha = bestMove.Evaluation;
                    }

                    if (alpha >= beta)
                    {
                        if (KillerMoves[absoluteDepth].Count < 2) { KillerMoves[absoluteDepth].Add(bestMove.Move); }
                        transpositionEntryType = TranspositionEntryType.Pruned;
                        break;
                    }
                }
                TranspositionTable[positionHash] = new Transposition(bestMove, depth, transpositionEntryType);
                bestMove.PV.Add(move);
                return bestMove;
            }

            //Search for black
            if (player == SideToMove.Black)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    curMove = possibleMoves[i];
                    Board.MovePiece(curMove, true);
                    if (i == 0)
                    {
                        curEval = AlphaBetaSearch(curMove, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, beta, player.ToOppositeColor(), pVStack);
                    }
                    else
                    {
                        curEval = AlphaBetaSearch(curMove, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, beta - 1, beta, player.ToOppositeColor(), pVStack);
                        if (beta < curEval.Evaluation && curEval.Evaluation > alpha)
                        {
                            Board.RepetitionTable.PopPosition();
                            curEval = AlphaBetaSearch(curMove, depth - 1 + ExtendSearch(curMove, depth), absoluteDepth + 1, alpha, beta, player.ToOppositeColor(), pVStack);
                        }
                    }
                    Board.RepetitionTable.PopPosition();
                    Board.UndoLastMove(true);

                    if (curEval.Evaluation < bestMove.Evaluation)
                    {
                        bestMove.SetToNewMove(curMove, curEval.Evaluation, curEval.PV);
                        //Console.WriteLine($"info score cp {bestMove.Evaluation} pv {bestMove.Move.ToAlgebra()} depth {absoluteDepth}");
                    }

                    if (bestMove.Evaluation < beta)
                    {
                        beta = bestMove.Evaluation;
                    }

                    if (beta <= alpha)
                    {
                        if (KillerMoves[absoluteDepth].Count < 2) { KillerMoves[absoluteDepth].Add(bestMove.Move); }
                        transpositionEntryType = TranspositionEntryType.Pruned;
                        break;
                    }
                }
                //if (bestMove.Move is null) { bestMove.SetToNewMove(curEval.Move, curEval.Evaluation, curEval.PV); }
                TranspositionTable[positionHash] = new Transposition(bestMove, depth, transpositionEntryType);
                bestMove.PV.Add(move);
                return bestMove;
            }
            return new MoveEvaluation(bestMove.Move, (int)GameResult.Draw);
        }

        private int QuiessenceSearch(int alpha, int beta, SideToMove player)
        {
            //Trying negamax approach for this one
            //alpha is the minumum secured score for the current player, while beta is the same for the opponent player 

            //Repetition table not needed as captures can never lead to a repeated position

            //Also im avoiding using the transpotition table for now as it would fill quickly with positions that would rarely be found again
            //This also seems to be the consensus of the sources ive found so far but i will test this later

            //Evaluate the position and assigne it to alpha if we dont prune already
            //The reason we can set this as a minimum for alpha is because making a move is almost always better than not
            //Also not making a move is a decent approximation of all the quiet moves as well  
            int staticEvaluation = (int)player * PositionEvaluator.EvaluateCurrentPosition(Board);

            //Opponent has already secured a better score in another branch, so stop searching further
            if (staticEvaluation >= beta) { return staticEvaluation; }
            if (alpha < staticEvaluation) { alpha = staticEvaluation; }
            int bestValue = staticEvaluation;

            //Generate tactical moves
            List<GeneratedMove> possibleMoves = GetPossibleMoves();
            if (Board.IsCheck)
            {
                //All evading moves will need to be evaluated
                possibleMoves = OrderMoves(possibleMoves);
            }
            else
            {
                possibleMoves = FilterAndOrderTacticalMoves(possibleMoves);
            }

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                Board.MovePiece(possibleMoves[i], true);
                int score = -QuiessenceSearch(-beta, -alpha, player.ToOppositeColor());
                Board.UndoLastMove(true);

                //Opponent has already secured a better score in another branch, so stop searching further
                if (score >= beta) { return score; }

                if (score > bestValue) { bestValue = score; }
                //Update our minimum secured score for this branch
                if (score > alpha) { alpha = score; }
            }
            return bestValue;
        }
        //position startpos moves d2d4 d7d5 c1f4 c8f5 b1d2 a7a5 g1f3 b8c6 a2a3 h7h5 h2h4 g8f6 b2b3 f6g4 g2g3 b7b5 f1g2 e7e6 c2c3 f8d6 d2f1 e8g8 f1d2 d6f4 g3f4 g4f6 e1g1 g8h8 f3g5 f6g4 d2f3 f7f6 g5h3 e6e5 d1d2 e5e4 f3e1 d8d6 d2c1 a8a7 f2f3 g4h6 f3e4 d5e4 c1c2 d6d5 g1h1 f8f7 h3f2 h6g4 f2e4 g4e3 e4g5 d5d7 g5f7 d7f7 c2a2 e3f1 g2c6 f1e3 a2d2 f7e6 c6f3 g7g6 e1d3 c7c6 d3c5 e6e8 h1g1 a7c7 a3a4 e3c2 a1a2 e8e3 d2e3 c2e3 a4b5 c7a7 f3c6 h8g8 b5b6 f5b1 b6a7 b1a2 a7a8q g8g7 a8a5 a2b1 a5d8 e3g4 e2e4 g7h7 e4e5 f6f5 d8d6 g4e3 c6e8 g6g5 f4g5 f5f4 e8h5 b1f5 d6f8 f4f3 h5f3 f5b1 f8f4 e3f5 f4c1 b1a2 c1c2 h7g6 f3g4 a2b3 c2b3 f5g7

        private MoveEvaluation ResolveNoMoves(SideToMove player)
        {
            GeneratedMove noMove = new GeneratedMove();
            //Mate
            if (Board.IsCheck)
            {
                Board.GameResult = player == SideToMove.White ? GameResult.BlackWin : GameResult.WhiteWin;
                return new MoveEvaluation(noMove, 1_000_000 * (int)Board.GameResult);
            }
            //Stalemate
            else
            {
                Board.GameResult = GameResult.Draw;
                return new MoveEvaluation(noMove, 0);
            }
        }

        //Makes sure that Quiessence Search doesnt start with a check
        private int ExtendSearch(GeneratedMove move, int depth)
        {
            if (depth == 0)
            {
                int extension = 0;
                if (move.IsCheck || Board.IsCheck)
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
        public MoveEvaluation GetNextMove(CancellationToken stopToken)
        {
            List<GeneratedMove> generatedMoves = GetPossibleMoves();
            MoveEvaluation bestMove = new MoveEvaluation(null, int.MinValue);
            if (generatedMoves.Count > 0)
            {
                NodeCounter = 0;
                for (int depth = 1; depth <= 4; depth++)
                {
                    bestMove = AlphaBetaSearch(new GeneratedMove(), depth, 0, int.MinValue, int.MaxValue, Board.SideToMove, bestMove.PV);
                    Console.Write($"info score cp {bestMove.Evaluation / 100} depth {depth} nodes {NodeCounter} pv ");
                    for (int j = bestMove.PV.Count - 1; j >= 0; j--)
                    {
                        Console.Write($"{(bestMove.PV[j].Piece != NormalPiece.Empty ? bestMove.PV[j].ToAlgebra() : "")} ");
                    }
                    Console.WriteLine();
                    if (stopToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            if (bestMove.Move is null)
            {
                Board.PrintBoard(SideToMove.White);
                throw new Exception("$GAME OVER");
            }
            return bestMove;
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


        //Dummy function for testing right now
        public void AnalyzeCurrent(AnalyzedPosition analyzedPosition)
        {
            analyzedPosition.BestMove = GetNextMove(analyzedPosition.StopToken);
            analyzedPosition.IsDone = true;
            return;
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