namespace Hattin.Types
{
    public class Move
    {
        public NormalPiece Piece { get; }
        public BoardSquare FromSquare { get; }
        public BoardSquare ToSquare { get; }

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