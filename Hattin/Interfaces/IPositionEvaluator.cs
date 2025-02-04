using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IPositionEvaluator
    {
        int EvaluatePositionAfterMove(Move newMove, BoardState currentBoard);
        int EvaluateCurrentPosition(BoardState currentBoard);
    }
}