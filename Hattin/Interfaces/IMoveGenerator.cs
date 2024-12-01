using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator : INormalPieceMoves
    {
        Move GenerateNextValidMove(BoardState currentBoard);
        Move GenerateNextCapture(BoardState currentBoard);
        Move GenerateNextCheck(BoardState currentBoard);
    }
}