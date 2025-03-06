namespace HattinEngineLibrary.Extensions.NormalPiece
{
    public static class NormalPieceExtensions
    {
        public static Types.SideToMove ToColor(this Types.NormalPiece piece)
        {
            if (piece >= Types.NormalPiece.BlackPawn && piece <= Types.NormalPiece.BlackKing)
            {
                return Types.SideToMove.Black;
            }
            else if (piece >= Types.NormalPiece.WhitePawn && piece <= Types.NormalPiece.WhiteKing)
            {
                return Types.SideToMove.White;
            }
            else
            {
                return Types.SideToMove.None;
            }
        }

        public static Types.NormalPieceValue ToValue(this Types.NormalPiece piece)
        {
            if (piece == Types.NormalPiece.WhitePawn || piece == Types.NormalPiece.BlackPawn)
            {
                return Types.NormalPieceValue.Pawn;
            }
            if (piece == Types.NormalPiece.WhiteKnight || piece == Types.NormalPiece.BlackKnight)
            {
                return Types.NormalPieceValue.Knight;
            }
            if (piece == Types.NormalPiece.WhiteBishop || piece == Types.NormalPiece.BlackBishop)
            {
                return Types.NormalPieceValue.Bishop;
            }
            if (piece == Types.NormalPiece.WhiteRook || piece == Types.NormalPiece.BlackRook)
            {
                return Types.NormalPieceValue.Rook;
            }
            if (piece == Types.NormalPiece.WhiteQueen || piece == Types.NormalPiece.BlackQueen)
            {
                return Types.NormalPieceValue.Queen;
            }
            if (piece == Types.NormalPiece.WhiteKing || piece == Types.NormalPiece.BlackKing)
            {
                return Types.NormalPieceValue.King;
            }
            return Types.NormalPieceValue.Empty;
        }
    }
}