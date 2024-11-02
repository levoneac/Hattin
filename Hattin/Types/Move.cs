namespace Hattin.Types
{
    public readonly struct Move
    {
        public NormalPiece Piece { get; init; }
        public BoardSquare FromSquare { get; init; }
        public BoardSquare ToSquare { get; init; }

        public Move()
        {
            Piece = NormalPiece.Empty;
            FromSquare = BoardSquare.NoSquare;
            ToSquare = BoardSquare.NoSquare;
        }

        public Move(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare)
        {
            Piece = piece;
            FromSquare = fromSquare;
            ToSquare = toSquare;
        }
    }
}