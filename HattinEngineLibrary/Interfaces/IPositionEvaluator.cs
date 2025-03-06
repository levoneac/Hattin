using HattinEngineLibrary.Types;

namespace HattinEngineLibrary.Interfaces
{
    public interface IPositionEvaluator
    {
        int EvaluatePositionAfterMove(Move newMove, BoardState currentBoard);
        int EvaluateCurrentPosition(BoardState currentBoard);
    }
}