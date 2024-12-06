namespace Hattin.Types
{
    public static class NormalPiecePromotionSquares
    {
        public static readonly BoardSquare[] WhitePawn = [BoardSquare.A8, BoardSquare.B8, BoardSquare.C8, BoardSquare.D8, BoardSquare.E8, BoardSquare.F8, BoardSquare.G8, BoardSquare.H8];
        public static readonly BoardSquare[] BlackPawn = [BoardSquare.A1, BoardSquare.B1, BoardSquare.C1, BoardSquare.D1, BoardSquare.E1, BoardSquare.F1, BoardSquare.G1, BoardSquare.H1];
        public static BoardSquare[] GetPromotionSquareFromNormalPiece(NormalPiece piece)
        {
            switch (piece)
            {
                case NormalPiece.WhitePawn:
                    return WhitePawn;
                case NormalPiece.BlackPawn:
                    return BlackPawn;

                default:
                    return [];
            }
        }

        public static BoardSquare[] GetPromotionSquareFromSideToMove(SideToMove color)
        {
            switch (color)
            {
                case SideToMove.White:
                    return WhitePawn;
                case SideToMove.Black:
                    return BlackPawn;

                default:
                    return [];
            }
        }
    }
}