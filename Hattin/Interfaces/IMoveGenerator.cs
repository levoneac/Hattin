using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator
    {
        List<AttackProjection> GenerateAllAttackedSquares();
        List<GeneratedMove> GenerateAllLegalMoves();
        GeneratedMove GenerateNextValidMove();
        GeneratedMove GenerateNextCapture();
        GeneratedMove GenerateNextCheck();
    }
}