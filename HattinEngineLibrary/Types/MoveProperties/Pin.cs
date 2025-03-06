namespace HattinEngineLibrary.Types
{
    public record Pin
    {
        public BoardSquare PinnedByPieceSquare { get; init; }
        public NormalPiece PinnedByPiece { get; init; }
        public BoardSquare PinnedPieceSquare { get; init; }
        public NormalPiece PinnedPiece { get; init; }
        public BoardSquare PinnedAgainstSquare { get; init; }
        public NormalPiece PinnedAgainstPiece { get; init; }
        public bool IsAbsolute { get; init; }
        public List<BoardSquare> AllowedSquares { get; init; }
        public bool EnPassantPin { get; init; }

        public Pin(BoardSquare pinnedByPieceSquare, NormalPiece pinnedByPiece, BoardSquare pinnedPieceSquare, NormalPiece pinnedPiece,
            BoardSquare pinnedAgainstSquare, NormalPiece pinnedAgainstPiece, bool isAbsolute, List<BoardSquare> allowedSquares, bool enPassantPin = false)
        {
            PinnedByPieceSquare = pinnedByPieceSquare;
            PinnedByPiece = pinnedByPiece;
            PinnedPieceSquare = pinnedPieceSquare;
            PinnedPiece = pinnedPiece;
            PinnedAgainstSquare = pinnedAgainstSquare;
            PinnedAgainstPiece = pinnedAgainstPiece;
            IsAbsolute = isAbsolute;
            AllowedSquares = allowedSquares;
            EnPassantPin = enPassantPin;
        }
    }
}