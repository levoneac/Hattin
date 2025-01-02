using Hattin.Extensions.NormalPiece;
using Hattin.Interfaces;
using Hattin.Types;
using Hattin.Utils;

namespace Hattin.Engine
{
    public class HattinEngine0_1
    {
        public BoardState Board { get; private set; }
        public IMoveGenerator MoveGenerator { get; private set; }
        public IMoveConstraintBuilder MoveConstraintBuilder { get; private set; }
        public IPositionEvaluator PositionEvaluator { get; private set; }
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

        //Testing move gen
        public void PlayNextMove()
        {
            //Make incremental later
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

        public void PlayUntillPly(int plyCount)
        {
            while (Board.PlyCounter <= plyCount)
            {
                PlayNextMove();
                Board.PrintBoard(SideToMove.White);
            }
        }
    }
}