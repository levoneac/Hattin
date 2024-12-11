using Hattin.Types;

namespace Hattin.Extensions.NormalPiece
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
    }
}