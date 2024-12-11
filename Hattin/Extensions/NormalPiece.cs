using Hattin.Types;

namespace Hattin.Extensions.NormalPiece
{
    public static class NormalPieceExtensions
    {
        public static SideToMove ToColor(this Types.NormalPiece piece)
        {
            if (piece >= Types.NormalPiece.BlackPawn && piece <= Types.NormalPiece.BlackKing)
            {
                return SideToMove.Black;
            }
            else if (piece >= Types.NormalPiece.WhitePawn && piece <= Types.NormalPiece.WhiteKing)
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