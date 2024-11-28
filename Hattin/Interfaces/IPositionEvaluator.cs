using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IPositionEvaluator
    {
        float EvaluatePositionAfterMove(Move newMove, BoardState currentBoard);
        float EvaluateCurrentPosition(BoardState currentBoard);
    }
}