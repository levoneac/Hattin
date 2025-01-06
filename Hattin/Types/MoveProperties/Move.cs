using Hattin.Extensions.NormalPiece;

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

        public static Move GetMoveFromAlgebra(string move, PieceList currentPieceProperties)
        {
            //todo: add promotion
            string fromString = move[0..2];
            string ToString = move[2..4];
            string promotion = "e";
            if (move.Length == 5)
            {
                promotion = move[4].ToString();
            }

            if (!Enum.TryParse(typeof(BoardSquare), fromString, true, out object? from))
            {
                throw new ArgumentException($"The given string is not valid", nameof(move));
            }
            if (!Enum.TryParse(typeof(BoardSquare), ToString, true, out object? to))
            {
                throw new ArgumentException($"The given string is not valid", nameof(move));
            }
            if (!Enum.TryParse(typeof(FENSymbols), promotion, true, out object? promote))
            {
                throw new ArgumentException($"The given string is not valid", nameof(move));
            }
            NormalPiece piece = currentPieceProperties.GetPieceOnSquare((BoardSquare)from);


            if ((NormalPiece)promote != NormalPiece.Empty)
            {
                NormalPiece[] promoteToPiece = NormalPieceClassifications.GetPiececlassFromPiece((NormalPiece)promote);
                NormalPiece promoteTo = piece.ToColor() == SideToMove.White ? promoteToPiece[0] : promoteToPiece[1];
                return new Move(piece, (BoardSquare)from, (BoardSquare)to, promoteTo: promoteTo);
            }
            return new Move(piece, (BoardSquare)from, (BoardSquare)to);
        }
    }
}