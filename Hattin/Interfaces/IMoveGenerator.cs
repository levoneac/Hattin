using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator : INormalPieceMoves
    {
        Move GenerateNextValidMove();
        Move GenerateNextCapture();
        Move GenerateNextCheck();
    }
}