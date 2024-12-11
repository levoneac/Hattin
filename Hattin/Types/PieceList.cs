using System.Collections.ObjectModel;
using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.Squares;

namespace Hattin.Types
{
    public class PieceList
    {
        //each piecetype has an array of boardsquares that tells you where a piece like that can be found
        private List<BoardSquare>[] piecePositions;
        public ReadOnlyCollection<ReadOnlyCollection<BoardSquare>> PiecePositions
        {
            get { return piecePositions.Select(list => list.AsReadOnly()).ToList().AsReadOnly(); } //looks slow ngl
        }

        //Tracks which color has a piece on each square
        private List<SideToMove> captureAndBlockingSquares;

        public List<BitBoard>[] PiecePositionsBitBoard { get; set; }
        public int NumPieces { get; private set; }
        public PieceList()
        {
            NumPieces = Enum.GetNames(typeof(NormalPiece)).Length; //make this take an interface? so that its not that tightly connected with NormalPiece
            piecePositions = new List<BoardSquare>[NumPieces];
            captureAndBlockingSquares = new List<SideToMove>();
            PiecePositionsBitBoard = new List<BitBoard>[NumPieces];
            for (int i = 0; i < NumPieces; i++)
            {
                piecePositions[i] = new List<BoardSquare>();
                //PiecePositionsBitBoard[i] = new List<BitBoard>();
            }

            for (int i = 0; i < 64; i++)
            {
                captureAndBlockingSquares.Add(SideToMove.None);
            }
        }

        public List<BoardSquare> GetPiecePositions(NormalPiece pieceType)
        {
            return piecePositions[(int)pieceType];
        }

        public SideToMove GetColorOfPieceOnSquare(BoardSquare square)
        {
            int arrayPos = square.ToBase64Int();
            return captureAndBlockingSquares[arrayPos];
        }

        //assumes that move is already verified from caller
        public void AddPiece(NormalPiece piece, BoardSquare square)
        {
            piecePositions[(int)piece].Add(square);

            int squareArrayPos = square.ToBase64Int();
            captureAndBlockingSquares[squareArrayPos] = piece.ToColor();
        }

        //assumes that move is already verified from caller
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

            int fromSquareArrayPos = fromSquare.ToBase64Int();
            int toSquareArrayPos = toSquare.ToBase64Int();

            captureAndBlockingSquares[fromSquareArrayPos] = SideToMove.None;
            captureAndBlockingSquares[toSquareArrayPos] = piece.ToColor();
        }

        //assumes that move is already verified from caller
        public void RemovePiece(NormalPiece piece, BoardSquare square)
        {
            if (!piecePositions[(int)piece].Remove(square))
            {
                throw new ArgumentOutOfRangeException(nameof(piece), $"There is no {piece} on square {square}");
            }

            int squareArrayPos = square.ToBase64Int();
            captureAndBlockingSquares[squareArrayPos] = SideToMove.None;
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
            for (int i = 0; i < 63; i++)
            {
                captureAndBlockingSquares[i] = SideToMove.None;
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