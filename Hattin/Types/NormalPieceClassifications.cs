namespace Hattin.Types
{
    public static class NormalPieceClassifications
    {

        public static NormalPiece[] Kings = [NormalPiece.WhiteKing, NormalPiece.BlackKing];
        public static NormalPiece[] Queens = [NormalPiece.WhiteQueen, NormalPiece.BlackQueen];
        public static NormalPiece[] Rooks = [NormalPiece.WhiteRook, NormalPiece.BlackRook];
        public static NormalPiece[] Bishops = [NormalPiece.WhiteBishop, NormalPiece.BlackBishop];
        public static NormalPiece[] Knights = [NormalPiece.WhiteKnight, NormalPiece.BlackKnight];
        public static NormalPiece[] Pawns = [NormalPiece.WhitePawn, NormalPiece.BlackPawn];
        public static NormalPiece[] Promoteable = [.. Knights, .. Bishops, .. Rooks, .. Queens];
        public static NormalPiece[] SlidingPieces = [.. Bishops, .. Rooks, .. Queens];
        public static NormalPiece[] JumpingPieces = [.. Knights, .. Kings];
        public static NormalPiece[] PawnMoves = Pawns;

        public static NormalPiece[] GetMovementfuncFromPiece(NormalPiece piece)
        {
            if (piece == NormalPiece.WhitePawn || piece == NormalPiece.BlackPawn)
            {
                return PawnMoves;
            }
            if (piece == NormalPiece.WhiteKnight || piece == NormalPiece.BlackKnight)
            {
                return JumpingPieces;
            }
            if (piece == NormalPiece.WhiteBishop || piece == NormalPiece.BlackBishop)
            {
                return SlidingPieces;
            }
            if (piece == NormalPiece.WhiteRook || piece == NormalPiece.BlackRook)
            {
                return SlidingPieces;
            }
            if (piece == NormalPiece.WhiteQueen || piece == NormalPiece.BlackQueen)
            {
                return SlidingPieces;
            }
            if (piece == NormalPiece.WhiteKing || piece == NormalPiece.BlackKing)
            {
                return JumpingPieces;
            }
            return [];
        }
    }
}