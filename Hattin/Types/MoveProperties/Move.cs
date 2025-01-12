using Hattin.Extensions.NormalPiece;
using Hattin.Utils;

namespace Hattin.Types
{
    //Changed from class to record because a chessmove is more like a value type, but too complex to be a struct(which should represent single values according to microsoft)
    //This also means its possible to check if two moves are equal based on their properties rather than their reference (possible with class as well i think, but you have to overload the Equals method)
    public record Move
    {
        public NormalPiece Piece { get; init; }
        public BoardSquare FromSquare { get; init; }
        public BoardSquare DestSquare { get; init; }
        public NormalPiece PromoteTo { get; set; }
        public BoardSquare RookCastleFromSquare { get; set; }
        public BoardSquare RookCastleToSquare { get; set; }
        //The empty square
        public BoardSquare EnPassantSquare { get; set; }
        //The square with the pawn
        public BoardSquare EnPassantCaptureSquare { get; set; }

        public Move()
        {
            Piece = NormalPiece.Empty;
            FromSquare = BoardSquare.NoSquare;
            DestSquare = BoardSquare.NoSquare;
            PromoteTo = NormalPiece.Empty;
            RookCastleFromSquare = BoardSquare.NoSquare;
            RookCastleToSquare = BoardSquare.NoSquare;
            EnPassantSquare = BoardSquare.NoSquare;
            EnPassantCaptureSquare = BoardSquare.NoSquare;
        }

        public Move(NormalPiece piece, BoardSquare fromSquare, BoardSquare destSquare, BoardSquare rookCastleFromSquare = BoardSquare.NoSquare, BoardSquare rookCastleToSquare = BoardSquare.NoSquare,
            NormalPiece promoteTo = NormalPiece.Empty, BoardSquare enPassantSquare = BoardSquare.NoSquare, BoardSquare enPassantCaptureSquare = BoardSquare.NoSquare)
        {
            Piece = piece;
            FromSquare = fromSquare;
            DestSquare = destSquare;
            RookCastleFromSquare = rookCastleFromSquare;
            RookCastleToSquare = rookCastleToSquare;
            PromoteTo = promoteTo;
            EnPassantSquare = enPassantSquare;
            EnPassantCaptureSquare = enPassantCaptureSquare;
        }

        public static Move GetMoveFromAlgebra(string move, BoardState board)
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
            NormalPiece piece = board.PieceProperties.GetPieceOnSquare((BoardSquare)from);
            BoardSquare fromSquare = (BoardSquare)from;
            BoardSquare toSquare = (BoardSquare)to;

            //Handle promotion
            if ((NormalPiece)promote != NormalPiece.Empty)
            {
                NormalPiece[] promoteToPiece = NormalPieceClassifications.GetPiececlassFromPiece((NormalPiece)promote);
                NormalPiece promoteTo = piece.ToColor() == SideToMove.White ? promoteToPiece[0] : promoteToPiece[1];
                return new Move(piece, fromSquare, toSquare, promoteTo: promoteTo);
            }

            //Handle castle
            BoardSquare rookCastleFromSquare = BoardSquare.NoSquare;
            BoardSquare rookCastleToSquare = BoardSquare.NoSquare;
            if (piece == NormalPiece.WhiteKing && fromSquare == BoardSquare.E1)
            {
                if (toSquare == BoardSquare.G1)
                {
                    rookCastleFromSquare = BoardSquare.H1;
                    rookCastleToSquare = BoardSquare.F1;
                }
                else if (toSquare == BoardSquare.C1)
                {
                    rookCastleFromSquare = BoardSquare.A1;
                    rookCastleToSquare = BoardSquare.D1;
                }
            }
            else if (piece == NormalPiece.BlackKing && fromSquare == BoardSquare.E8)
            {
                if (toSquare == BoardSquare.G8)
                {
                    rookCastleFromSquare = BoardSquare.H8;
                    rookCastleToSquare = BoardSquare.F8;
                }
                else if (toSquare == BoardSquare.C8)
                {
                    rookCastleFromSquare = BoardSquare.A8;
                    rookCastleToSquare = BoardSquare.D8;
                }
            }
            if (rookCastleToSquare != BoardSquare.NoSquare && rookCastleFromSquare != BoardSquare.NoSquare)
            {
                return new Move(piece, fromSquare, toSquare, rookCastleToSquare: rookCastleToSquare, rookCastleFromSquare: rookCastleFromSquare);
            }

            //Handle set enpassant square
            BoardSquare enPassantSquare = BoardSquare.NoSquare;
            BoardSquare enPassantCaptureSquare = BoardSquare.NoSquare;
            if (piece == NormalPiece.WhitePawn && NormalPieceStartingSquares.WhitePawn.Contains(fromSquare))
            {
                enPassantSquare = SquareRange.GetSquaresBetween(fromSquare, toSquare, false).FirstOrDefault(BoardSquare.NoSquare);
            }
            else if (piece == NormalPiece.BlackPawn && NormalPieceStartingSquares.BlackPawn.Contains(fromSquare))
            {
                enPassantSquare = SquareRange.GetSquaresBetween(fromSquare, toSquare, false).FirstOrDefault(BoardSquare.NoSquare);
            }

            //Handle enpassant capture
            if (piece.ToValue() == NormalPieceValue.Pawn && toSquare == board.EnPassantSquare)
            {
                enPassantCaptureSquare = SquareRange.GetEnPassantCaptureSquare(fromSquare, toSquare);
            }

            //standard move
            return new Move(piece, fromSquare, toSquare, enPassantSquare: enPassantSquare, enPassantCaptureSquare: enPassantCaptureSquare);
        }

        public static string ToAlgebra(Move move)
        {
            string promoteTo = "";
            if (move.PromoteTo != NormalPiece.Empty)
            {
                promoteTo = ((FENSymbols)move.PromoteTo).ToString().ToLower();
            }
            return $"{move.FromSquare.ToString().ToLower()}{move.DestSquare.ToString().ToLower()}{promoteTo}";
        }
    }
}