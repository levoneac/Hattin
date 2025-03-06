namespace HattinEngineLibrary.Types
{
    public static class NormalPieceStartingSquares
    {
        public static readonly BoardSquare[] WhitePawn = [BoardSquare.A2, BoardSquare.B2, BoardSquare.C2, BoardSquare.D2, BoardSquare.E2, BoardSquare.F2, BoardSquare.G2, BoardSquare.H2];
        public static readonly BoardSquare[] BlackPawn = [BoardSquare.A7, BoardSquare.B7, BoardSquare.C7, BoardSquare.D7, BoardSquare.E7, BoardSquare.F7, BoardSquare.G7, BoardSquare.H7];
        public static readonly BoardSquare[] WhiteRook = [BoardSquare.A1, BoardSquare.H1];
        public static readonly BoardSquare[] BlackRook = [BoardSquare.A8, BoardSquare.H8];
        public static readonly BoardSquare[] WhiteKing = [BoardSquare.E1];
        public static readonly BoardSquare[] BlackKing = [BoardSquare.E8];

        public static BoardSquare[] GetStartingSquareFromNormalPiece(NormalPiece piece)
        {
            switch (piece)
            {
                case NormalPiece.WhitePawn:
                    return WhitePawn;
                case NormalPiece.BlackPawn:
                    return BlackPawn;

                case NormalPiece.WhiteRook:
                    return WhiteRook;
                case NormalPiece.BlackRook:
                    return BlackRook;

                case NormalPiece.WhiteKing:
                    return WhiteKing;
                case NormalPiece.BlackKing:
                    return BlackKing;

                default:
                    return [];
            }

        }
    }


}