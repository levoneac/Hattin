namespace Hattin.Types
{
    public class Move
    {
        public NormalPiece Piece { get; init; }
        public BoardSquare FromSquare { get; init; }
        public BoardSquare DestSquare { get; init; }
        public NormalPiece PromoteTo { get; set; }
        public BoardSquare RookCastleSquare { get; init; }

        public Move()
        {
            Piece = NormalPiece.Empty;
            FromSquare = BoardSquare.NoSquare;
            DestSquare = BoardSquare.NoSquare;
            PromoteTo = NormalPiece.Empty;
            RookCastleSquare = BoardSquare.NoSquare;
        }

        public Move(NormalPiece piece, BoardSquare fromSquare, BoardSquare destSquare, BoardSquare rookCastleSquare = BoardSquare.NoSquare, NormalPiece promoteTo = NormalPiece.Empty)
        {
            Piece = piece;
            FromSquare = fromSquare;
            DestSquare = destSquare;
            RookCastleSquare = rookCastleSquare;
            PromoteTo = promoteTo;
        }
    }
}