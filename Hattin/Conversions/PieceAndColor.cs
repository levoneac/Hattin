using Hattin.Types;

namespace Hattin.Conversions
{
    public static class PieceAndColor
    {
        public static SideToMove PieceToColor(NormalPiece piece)
        {
            if (piece >= NormalPiece.BlackPawn && piece <= NormalPiece.BlackKing)
            {
                return SideToMove.Black;
            }
            else if (piece >= NormalPiece.WhitePawn && piece <= NormalPiece.WhiteKing)
            {
                return SideToMove.White;
            }
            else
            {
                return SideToMove.None;
            }
        }
    }
}