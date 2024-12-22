using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator : INormalPieceMoves
    {
        List<AttackProjection> GenerateAllAttackedSquares();
        List<GeneratedMove> GenerateAllLegalMoves(List<Func<GeneratedMove, bool>>? constraints = null);
        GeneratedMove GenerateNextValidMove();
        GeneratedMove GenerateNextCapture();
        GeneratedMove GenerateNextCheck();
    }
}