namespace Hattin.Types
{
    public enum NormalPieceValue : int
    {
        Empty = 0,
        Pawn = 1_000,
        Knight = 3_000,
        Bishop = 3_250,
        Rook = 5_100,
        Queen = 10_000,
        King = 1_000_000
    }
}