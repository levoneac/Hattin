namespace Hattin.Types
{
    public readonly struct Pin
    {
        public readonly BoardSquare PinnedByPieceSquare { get; init; }
        public readonly NormalPiece PinnedByPiece { get; init; }
        public readonly BoardSquare PinnedPieceSquare { get; init; }
        public readonly NormalPiece PinnedPiece { get; init; }
        public readonly BoardSquare PinnedAgainstSquare { get; init; }
        public readonly NormalPiece PinnedAgainstPiece { get; init; }
        public readonly bool IsAbsolute { get; init; }
        public readonly BoardSquare[] AllowedSquares { get; init; }

        public Pin(BoardSquare pinnedByPieceSquare, NormalPiece pinnedByPiece, BoardSquare pinnedPieceSquare, NormalPiece pinnedPiece,
            BoardSquare pinnedAgainstSquare, NormalPiece pinnedAgainstPiece, bool isAbsolute, BoardSquare[] allowedSquares)
        {
            PinnedByPieceSquare = pinnedByPieceSquare;
            PinnedByPiece = pinnedByPiece;
            PinnedPieceSquare = pinnedPieceSquare;
            PinnedPiece = pinnedPiece;
            PinnedAgainstSquare = pinnedAgainstSquare;
            PinnedAgainstPiece = pinnedAgainstPiece;
            IsAbsolute = isAbsolute;
            AllowedSquares = allowedSquares;
        }
    }
}