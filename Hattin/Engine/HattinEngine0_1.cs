using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Engine
{
    public class HattinEngine0_1
    {
        public BoardState Board { get; private set; }
        public IMoveGenerator MoveGenerator { get; private set; }
        public IPositionEvaluator PositionEvaluator { get; private set; }
        public HattinEngine0_1(BoardState board, IMoveGenerator moveGenerator, IPositionEvaluator positionEvaluator)
        {
            Board = board;
            MoveGenerator = moveGenerator;
            PositionEvaluator = positionEvaluator;
        }

        private List<Func<GeneratedMove, bool>> GetConstraintFuncs()
        {
            List<Func<GeneratedMove, bool>> funcs = new List<Func<GeneratedMove, bool>>();

            if (Board.IsCheck)
            {
                funcs.Add(StopCheck);
            }

            return funcs;


            bool StopCheck(GeneratedMove move)
            {
                return true;
            }
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