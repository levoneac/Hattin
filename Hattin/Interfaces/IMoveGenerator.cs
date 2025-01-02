using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveGenerator : INormalPieceMoves
    {
        //A list of move-sequences(f.ex. one list contains queen from A1 to H1 and another contains queen from A1 to A8),
        //which each hold a list of AttackProjections(information about the interaction on that square)
        List<List<AttackProjection>> GenerateAllAttackedSquares();
        List<GeneratedMove> GenerateAllLegalMoves(Func<List<GeneratedMove>, List<GeneratedMove>>? constraints = null);
        GeneratedMove GenerateNextValidMove();
        GeneratedMove GenerateNextCapture();
        GeneratedMove GenerateNextCheck();
    }
}