using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator
    {
        Move GetNextValidMove(BoardState currentBoard);
        Move GetNextCapture(BoardState currentBoard);
        Move GetNextCheck(BoardState currentBoard);
    }
}