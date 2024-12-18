using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator
    {
        List<GeneratedMove> GenerateAllLegalMoves();
        GeneratedMove GenerateNextValidMove();
        GeneratedMove GenerateNextCapture();
        GeneratedMove GenerateNextCheck();
    }
}