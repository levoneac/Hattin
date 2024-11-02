namespace Hattin.Types
{
    //WIP, need to see usecase first
    public class PieceList
    {
        public List<BoardSquare>[] PiecePositions { get; set; }
        public int NumPieces { get; private set; }
        public PieceList()
        {
            NumPieces = Enum.GetNames(typeof(NormalPiece)).Length;
            PiecePositions = new List<BoardSquare>[NumPieces];
            for (int i = 0; i < NumPieces; i++)
            {
                PiecePositions[i] = new List<BoardSquare>();
            }
        }

        public PieceTotals CalculatePieceTotals()
        {
            int blackTotal = 0;
            int whiteTotal = 0;

            for (int i = 0; i < PiecePositions.Length; i++)
            {
                int amountOfPiece = PiecePositions[i].Count;
                switch ((NormalPiece)i)
                {
                    case NormalPiece.BlackPawn:
                        blackTotal += (int)NormalPieceValues.Pawn * amountOfPiece;
                        break;
                    case NormalPiece.WhitePawn:
                        whiteTotal += (int)NormalPieceValues.Pawn * amountOfPiece;
                        break;

                    case NormalPiece.BlackKnight:
                        blackTotal += (int)NormalPieceValues.Knight * amountOfPiece;
                        break;
                    case NormalPiece.WhiteKnight:
                        whiteTotal += (int)NormalPieceValues.Knight * amountOfPiece;
                        break;

                    case NormalPiece.BlackBishop:
                        blackTotal += (int)NormalPieceValues.Bishop * amountOfPiece;
                        break;
                    case NormalPiece.WhiteBishop:
                        whiteTotal += (int)NormalPieceValues.Bishop * amountOfPiece;
                        break;

                    case NormalPiece.BlackRook:
                        blackTotal += (int)NormalPieceValues.Rook * amountOfPiece;
                        break;
                    case NormalPiece.WhiteRook:
                        whiteTotal += (int)NormalPieceValues.Rook * amountOfPiece;
                        break;

                    case NormalPiece.BlackQueen:
                        blackTotal += (int)NormalPieceValues.Queen * amountOfPiece;
                        break;
                    case NormalPiece.WhiteQueen:
                        whiteTotal += (int)NormalPieceValues.Queen * amountOfPiece;
                        break;
                }
            }
            return new PieceTotals(blackTotal, whiteTotal);
        }

        public void ClearPieceList()
        {
            foreach (List<BoardSquare> listOfPieces in PiecePositions)
            {
                listOfPieces.Clear();
            }
        }
    }

    public readonly struct PieceTotals
    {
        public readonly int black;
        public readonly int white;

        public PieceTotals(int blackSum, int whiteSum)
        {
            black = blackSum;
            white = whiteSum;
        }
    }
}