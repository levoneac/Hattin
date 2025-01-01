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

        private Func<List<GeneratedMove>, List<GeneratedMove>> GetConstraintFuncs()
        {
            MoveConstraintBuilder.Reset();
            if (Board.IsCheck)
            {
                MoveConstraintBuilder.SetStopCheck();
            }

            return MoveConstraintBuilder.GetConstraintFunction();
        }

        public void PlayNextMove()
        {
            if (Board.PieceProperties.AttackSquaresInitialized == false)
            {
                Board.PieceProperties.UpdateAllAttackSquares(MoveGenerator.GenerateAllAttackedSquares());
            }
            List<GeneratedMove> generatedMoves = MoveGenerator.GenerateAllLegalMoves(GetConstraintFuncs());
            GeneratedMove chosenMove = generatedMoves?[new Random().Next(0, generatedMoves.Count - 1)] ?? new GeneratedMove();
            Move outputMove = new Move(chosenMove.Piece, chosenMove.FromSquare, chosenMove.DestSquare);
            Board.MovePiece(outputMove);
        }

        public void PlayUntillPly(int plyCount)
        {
            while (Board.PlyCounter <= plyCount)
            {
                PlayNextMove();
            }
        }
    }
}