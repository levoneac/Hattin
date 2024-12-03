namespace Hattin.Types
{
    public static class NormalPieceOffsets
    {
        public static readonly int[] Pawn = [];
        public static readonly int[] Knight = [-21, -19, -12, -8, 8, 12, 19, 21];
        public static readonly int[] Bishop = [-11, -9, 9, 11];
        public static readonly int[] Rook = [-10, -1, 1, 10];
        public static readonly int[] Queen = [-11, -10, -9, -1, 1, 9, 10, 11];
        public static readonly int[] King = [-11, -10, -9, -1, 1, 9, 10, 11];


        public static int[] GetOffsetFromNormalPiece(NormalPiece piece)
        {
            if (piece == NormalPiece.WhitePawn || piece == NormalPiece.BlackPawn)
            {
                return Pawn;
            }
            if (piece == NormalPiece.WhiteKnight || piece == NormalPiece.BlackKnight)
            {
                return Knight;
            }
            if (piece == NormalPiece.WhiteBishop || piece == NormalPiece.BlackBishop)
            {
                return Bishop;
            }
            if (piece == NormalPiece.WhiteRook || piece == NormalPiece.BlackRook)
            {
                return Rook;
            }
            if (piece == NormalPiece.WhiteQueen || piece == NormalPiece.BlackQueen)
            {
                return Queen;
            }
            if (piece == NormalPiece.WhiteKing || piece == NormalPiece.BlackKing)
            {
                return King;
            }
            return [];
        }
    }
}