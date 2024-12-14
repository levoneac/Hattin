namespace Hattin.Types
{
    public static class NormalPieceMovement
    {
        public static NormalPiece[] SlidingPieces = [NormalPiece.WhiteBishop, NormalPiece.BlackBishop, NormalPiece.BlackRook, NormalPiece.WhiteRook, NormalPiece.WhiteQueen, NormalPiece.BlackQueen];
        public static NormalPiece[] JumpingPieces = [NormalPiece.WhiteKnight, NormalPiece.BlackKnight, NormalPiece.WhiteKing, NormalPiece.BlackKing];
        public static NormalPiece[] PawnMoves = [NormalPiece.WhitePawn, NormalPiece.BlackPawn];

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