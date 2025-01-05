using Hattin.Extensions.NormalPiece;
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
        public GeneratedMove GetNextMove()
        {
            Board.PieceProperties.UpdateAllAttackSquares(MoveGenerator.GenerateAllAttackedSquares());

            List<GeneratedMove> generatedMoves = MoveGenerator.GenerateAllLegalMoves(GetConstraintFuncs());
            GeneratedMove chosenMove;
            if (generatedMoves.Count > 0)
            {
                chosenMove = generatedMoves?[new Random().Next(0, generatedMoves.Count - 1)] ?? new GeneratedMove();
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