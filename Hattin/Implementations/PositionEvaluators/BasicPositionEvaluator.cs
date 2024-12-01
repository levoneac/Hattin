using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.PositionEvaluators
{
    public class BasicPositionEvaluator : IPositionEvaluator
    {
        public float EvaluateCurrentPosition(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public float EvaluatePositionAfterMove(Move newMove, BoardState currentBoard)
        {
            throw new NotImplementedException();
        }
    }
}