using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator : INormalPieceMoves
    {
        List<GeneratedMove> GeneratAllLegalMoves();
        GeneratedMove GenerateNextValidMove();
        GeneratedMove GenerateNextCapture();
        GeneratedMove GenerateNextCheck();
    }
}