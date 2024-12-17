namespace Hattin.Types
{
    public class Move
    {
        public NormalPiece Piece { get; init; }
        public BoardSquare FromSquare { get; init; }
        public BoardSquare DestSquare { get; init; }

        public Move()
        {
            Piece = NormalPiece.Empty;
            FromSquare = BoardSquare.NoSquare;
            DestSquare = BoardSquare.NoSquare;
        }

        public Move(NormalPiece piece, BoardSquare fromSquare, BoardSquare destSquare)
        {
            Piece = piece;
            FromSquare = fromSquare;
            DestSquare = destSquare;
        }
    }
}