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
        private TranspositionTable<float> TranspositionTable;
        public HattinEngine0_1(BoardState board, IMoveGenerator moveGenerator, IMoveConstraintBuilder moveConstraintBuilder, IPositionEvaluator positionEvaluator)
        {
            Board = board;
            MoveGenerator = moveGenerator;
            MoveConstraintBuilder = moveConstraintBuilder;
            PositionEvaluator = positionEvaluator;
            TranspositionTable = new TranspositionTable<float>(1_000_000);
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
        private MoveEvaluation AlphaBetaSearch(GeneratedMove move, BoardState currentBoard, int depth, int absoluteDepth, int maxDepth, float alpha, float beta, SideToMove player, TranspositionTable<float> transpositionTable)
        {
            if (depth == 0 || absoluteDepth >= maxDepth)
            {
                return new MoveEvaluation(move, PositionEvaluator.EvaluateCurrentPosition(currentBoard));
            }
            //if (move.IsCheck) { Board.IsCheck = true; } else { Board.IsCheck = false; } Runs into problems with discovery check

            MoveEvaluation curEval;
            float positionScore;
            GeneratedMove bestMove = new GeneratedMove();
            List<GeneratedMove> possibleMoves = GetPossibleMoves();

            if (possibleMoves.Count == 0)
            {
                return ResolveNoMoves(player);
            }
            if (player == SideToMove.White)
            {
                float bestValue = float.MinValue;
                foreach (GeneratedMove curMove in possibleMoves)
                {
                    Board.MovePiece(curMove);
                    int positionHash = Board.GetPositionHash();
                    if (transpositionTable.TryGetValue(positionHash, out float preCalculated))
                    {

                        positionScore = preCalculated;
                    }
                    else
                    {
                        curEval = AlphaBetaSearch(curMove, currentBoard, depth - 1 + ExtendSearch(curMove), absoluteDepth + 1, maxDepth, alpha, beta, player.ToOppositeColor(), transpositionTable);
                        positionScore = curEval.Evaluation;
                        transpositionTable[positionHash] = positionScore;
                    }

                    Board.UndoLastMove();
                    if (positionScore > bestValue)
                    {
                        bestValue = positionScore;
                        bestMove = curMove;
                    }

                    if (bestValue >= beta)
                    {
                        break;
                    }
                }
                return new MoveEvaluation(bestMove, bestValue);
            }

            if (player == SideToMove.Black)
            {
                float bestValue = float.MaxValue;
                foreach (GeneratedMove curMove in possibleMoves)
                {
                    Board.MovePiece(curMove);
                    int positionHash = Board.GetPositionHash();
                    if (transpositionTable.TryGetValue(positionHash, out float preCalculated))
                    {
                        positionScore = preCalculated;
                    }
                    else
                    {
                        curEval = AlphaBetaSearch(curMove, currentBoard, depth - 1 + ExtendSearch(curMove), absoluteDepth + 1, maxDepth, alpha, beta, player.ToOppositeColor(), transpositionTable);
                        positionScore = curEval.Evaluation;
                        transpositionTable[positionHash] = positionScore;
                    }
                    Board.UndoLastMove();
                    if (positionScore < bestValue)
                    {
                        bestValue = positionScore;
                        bestMove = curMove;
                    }

                    if (bestValue <= alpha)
                    {
                        break;
                    }
                }
                return new MoveEvaluation(bestMove, bestValue);
            }
            return new MoveEvaluation(bestMove, (int)GameResult.Draw);

        }

        private MoveEvaluation ResolveNoMoves(SideToMove player)
        {
            GeneratedMove noMove = new GeneratedMove();
            //Mate
            if (Board.IsCheck)
            {
                Board.GameResult = player == SideToMove.White ? GameResult.BlackWin : GameResult.WhiteWin;
                return new MoveEvaluation(noMove, 100000 * (int)Board.GameResult);
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

                chosenMove = AlphaBetaSearch(new GeneratedMove(), Board, 3, 0, 10, float.MinValue, float.MaxValue, Board.SideToMove, TranspositionTable).Move;
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