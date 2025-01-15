namespace Hattin.Types
{
    public static class NormalPieceSquareScores
    {
        public static readonly int[] WhitePawn = {0,0,0,0,0,0,0,0,
                                                    0,2,1,2,2,1,2,0,
                                                    20,10,10,10,10,10,10,20,
                                                    25,20,20,35,35,20,20,25,
                                                    35,30,30,45,45,30,30,35,
                                                    45,40,40,55,55,40,40,45,
                                                    70,60,60,75,75,60,60,70,
                                                    100,100,100,100,100,100,100,100 };
        public static readonly int[] BlackPawn = WhitePawn.Reverse().ToArray();

        public static readonly int[] WhiteKnight = {0,0,0,0,0,0,0,0,
                                                    10,10,10,10,10,10,10,10,
                                                    10,20,20,20,20,20,20,10,
                                                    10,30,30,30,30,30,30,10,
                                                    10,40,40,40,40,40,40,10,
                                                    10,50,50,50,50,50,50,10,
                                                    10,10,10,10,10,10,10,10,
                                                    0,0,0,0,0,0,0,0, };
        public static readonly int[] BlackKnight = WhiteKnight.Reverse().ToArray();

        public static readonly int[] WhiteBishop = {30,10,10,10,10,10,10,30,
                                                    10,30,10,10,10,10,30,10,
                                                    10,20,20,20,20,20,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    10,30,10,10,10,10,30,10,
                                                    30,10,10,10,10,10,10,30 };
        public static readonly int[] BlackBishop = WhiteBishop;

        public static readonly int[] WhiteRook = {20,20,20,20,20,20,20,20,
                                                    30,30,30,30,30,30,30,30,
                                                    20,20,20,20,20,20,20,20,
                                                    10,20,50,50,50,50,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    50,50,50,50,50,50,50,50,
                                                    40,40,40,40,40,40,40,40, };
        public static readonly int[] BlackRook = WhiteRook.Reverse().ToArray();

        public static readonly int[] WhiteQueen = {20,20,20,20,20,20,20,20,
                                                    30,30,30,30,30,30,30,30,
                                                    20,20,20,20,20,20,20,20,
                                                    10,20,50,50,50,50,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    10,20,50,50,50,50,20,10,
                                                    50,50,50,50,50,50,50,50,
                                                    40,40,40,40,40,40,40,40, };
        public static readonly int[] BlackQueen = WhiteQueen.Reverse().ToArray();

        public static readonly int[] WhiteKing = {70,70,0,5,0,30,70,70,
                                                    1,1,1,1,1,1,1,1,
                                                    1,1,1,1,1,1,1,1,
                                                    1,1,1,1,1,1,1,1,
                                                    1,1,1,1,1,1,1,1,
                                                    1,1,1,1,1,1,1,1,
                                                    1,1,1,1,1,1,1,1,
                                                    1,1,1,1,1,1,1,1, };
        public static readonly int[] BlackKing = WhiteKing.Reverse().ToArray();

        public static int[] GetPieceSquareScore(NormalPiece piece)
        {
            switch (piece)
            {
                case NormalPiece.WhitePawn:
                    return WhitePawn;
                case NormalPiece.BlackPawn:
                    return BlackPawn;

                case NormalPiece.WhiteKnight:
                    return WhiteKnight;
                case NormalPiece.BlackKnight:
                    return BlackKnight;

                case NormalPiece.WhiteBishop:
                    return WhiteBishop;
                case NormalPiece.BlackBishop:
                    return BlackBishop;

                case NormalPiece.WhiteRook:
                    return WhiteRook;
                case NormalPiece.BlackRook:
                    return BlackRook;

                case NormalPiece.WhiteQueen:
                    return WhiteQueen;
                case NormalPiece.BlackQueen:
                    return BlackQueen;

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





