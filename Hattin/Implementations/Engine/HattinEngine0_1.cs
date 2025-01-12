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
        public HattinEngine0_1(BoardState board, IMoveGenerator moveGenerator, IMoveConstraintBuilder moveConstraintBuilder, IPositionEvaluator positionEvaluator)
        {
            Board = board;
            MoveGenerator = moveGenerator;
            MoveConstraintBuilder = moveConstraintBuilder;
            PositionEvaluator = positionEvaluator;
        }

        //Gets the constraints based on the current boardstate
        private Func<List<GeneratedMove>, List<GeneratedMove>>? GetConstraintFuncs()
        {
            MoveConstraintBuilder.Reset();
            if (Board.IsCheck)
            {
                MoveConstraintBuilder.SetStopCheck();
            }
            MoveConstraintBuilder.SetPinRestriction();
            return MoveConstraintBuilder.GetConstraintFunction();
        }

        private MoveEvaluation AlphaBetaSearch(GeneratedMove move, BoardState currentBoard, int depth, int maxDepth, float alpha, float beta, SideToMove player)
        {
            if (depth == maxDepth)
            {
                return new MoveEvaluation(move, PositionEvaluator.EvaluateCurrentPosition(currentBoard));
            }
            MoveEvaluation curEval;
            GeneratedMove bestMove = new GeneratedMove();
            if (player == SideToMove.White)
            {
                float bestValue = float.MinValue;
                foreach (GeneratedMove curMove in GetPossibleMoves())
                {
                    Board.MovePiece(curMove);
                    curEval = AlphaBetaSearch(curMove, currentBoard, depth + 1, maxDepth, alpha, beta, player.ToOppositeColor());
                    Board.UndoLastMove();
                    if (curEval.Evaluation > bestValue)
                    {
                        bestValue = curEval.Evaluation;
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
                foreach (GeneratedMove curMove in GetPossibleMoves())
                {
                    Board.MovePiece(curMove);
                    curEval = AlphaBetaSearch(curMove, currentBoard, depth + 1, maxDepth, alpha, beta, player.ToOppositeColor());
                    Board.UndoLastMove();
                    if (curEval.Evaluation < bestValue)
                    {
                        bestValue = curEval.Evaluation;
                        bestMove = curMove;
                    }

                    if (bestValue <= alpha)
                    {
                        break;
                    }
                }
                return new MoveEvaluation(bestMove, bestValue);
            }
            return new MoveEvaluation(new GeneratedMove(), 0);
        }

        public List<GeneratedMove> GetPossibleMoves()
        {
            Board.PieceProperties.UpdateAllAttackSquares(MoveGenerator.GenerateAllAttackedSquares());
            if (Board.PieceProperties.GetCheckSource(Board.SideToMove).Count > 0)
            {
                Board.IsCheck = true;
            }
            else
            {
                Board.IsCheck = false;
            }
            return MoveGenerator.GenerateAllLegalMoves(GetConstraintFuncs());
        }

        //Mostly for testingpurposes still
        public GeneratedMove GetNextMove()
        {
            List<GeneratedMove> generatedMoves = GetPossibleMoves();
            GeneratedMove chosenMove;
            if (generatedMoves.Count > 0)
            {
                //chosenMove = generatedMoves?[new Random().Next(0, generatedMoves.Count - 1)] ?? new GeneratedMove();
                chosenMove = AlphaBetaSearch(new GeneratedMove(), Board, 0, 3, float.MinValue, float.MaxValue, Board.SideToMove).Move;
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