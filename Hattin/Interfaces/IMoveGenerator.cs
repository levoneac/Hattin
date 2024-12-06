using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator : INormalPieceMoves
    {
        GeneratedMove GenerateNextValidMove();
        GeneratedMove GenerateNextCapture();
        GeneratedMove GenerateNextCheck();
    }
}