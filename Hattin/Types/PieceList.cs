using System.Collections.ObjectModel;

namespace Hattin.Types
{
    //WIP, need to see usecase first
    public class PieceList
    {
        private List<BoardSquare>[] piecePositions;
        public ReadOnlyCollection<ReadOnlyCollection<BoardSquare>> PiecePositions
        {
            get { return piecePositions.Select(list => list.AsReadOnly()).ToList().AsReadOnly(); } //looks slow ngl
        }
        public List<BitBoard>[] PiecePositionsBitBoard { get; set; }
        public int NumPieces { get; private set; }
        public PieceList()
        {
            NumPieces = Enum.GetNames(typeof(NormalPiece)).Length;
            piecePositions = new List<BoardSquare>[NumPieces];
            PiecePositionsBitBoard = new List<BitBoard>[NumPieces];
            for (int i = 0; i < NumPieces; i++)
            {
                piecePositions[i] = new List<BoardSquare>();
                PiecePositionsBitBoard[i] = new List<BitBoard>();
            }
        }

        public void AddPiece(NormalPiece piece, BoardSquare square)
        {
            piecePositions[(int)piece].Add(square);
        }
        public void MovePiece(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare)
        {
            int indexOfFromSquare = piecePositions[(int)piece].IndexOf(fromSquare); //LINQ should be side effect free, so you cant change inplace afaik
            if (indexOfFromSquare == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(piece), $"There is no {piece} on square {fromSquare}");
            }
            //Should there be checks to see if this square is occupied by other pieces?
            //Only allow if no friendly piece and bool "capture" argument is true?
            piecePositions[(int)piece][indexOfFromSquare] = toSquare;
        }
        public void RemovePiece(NormalPiece piece, BoardSquare square)
        {
            if (!piecePositions[(int)piece].Remove(square))
            {
                throw new ArgumentOutOfRangeException(nameof(piece), $"There is no {piece} on square {square}");
            }
        }

        public PieceTotals CalculatePieceTotals()
        {
            int blackTotal = 0;
            int whiteTotal = 0;

            for (int i = 0; i < piecePositions.Length; i++)
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
            foreach (List<BoardSquare> listOfPieces in piecePositions)
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