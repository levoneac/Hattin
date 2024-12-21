using System.Collections.ObjectModel;
using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;

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
        private List<SideToMove> captureAndBlockingSquares; //switch to array
        private List<AttackInformation> attackSquares; //switch to array
        private NormalPiece[] squareContents;
        

        public List<BitBoard>[] PiecePositionsBitBoard { get; set; }
        public int NumPieces { get; private set; }
        public PieceList()
        {
            NumPieces = Enum.GetNames(typeof(NormalPiece)).Length;
            piecePositions = new List<BoardSquare>[NumPieces];
            PiecePositionsBitBoard = new List<BitBoard>[NumPieces];
            captureAndBlockingSquares = new List<SideToMove>(64);
            attackSquares = new List<AttackInformation>(64);
            squareContents = new NormalPiece[64];
            for (int i = 0; i < NumPieces; i++)
            {
                piecePositions[i] = new List<BoardSquare>();
                //PiecePositionsBitBoard[i] = new List<BitBoard>();
            }

            for (int i = 0; i < 64; i++)
            {
                captureAndBlockingSquares.Add(SideToMove.None);
                attackSquares.Add(new AttackInformation{ AttackTotals = new ColorCount(), Data = new List<AttackProjection>()});
                squareContents[i] = NormalPiece.Empty;
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

        public AttackInformation GetAttackCountOnSquare(BoardSquare square)
        {
            int arrayPos = square.ToBase64Int();
            return attackSquares[arrayPos];
        }

        public NormalPiece GetPieceOnSquare(BoardSquare square)
        {
            int arrayPos = square.ToBase64Int();
            return squareContents[arrayPos];
        }
        public void UpdateAllAttackSquares(List<AttackProjection> attackProjections)
        {
            AttackInformation curItem;
            foreach (AttackProjection attack in attackProjections)
            {
                curItem = attackSquares[attack.Square.ToBase64Int()];
                curItem.AttackTotals.IncrementColor(attack.AsSide);
                curItem.Data.Add(attack);
            }
        }

        //assumes that move is already verified from caller
        public void AddPiece(NormalPiece piece, BoardSquare square)
        {
            piecePositions[(int)piece].Add(square);

            int squareArrayPos = square.ToBase64Int();
            captureAndBlockingSquares[squareArrayPos] = piece.ToColor();
            squareContents[squareArrayPos] = piece;
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

            squareContents[fromSquareArrayPos] = NormalPiece.Empty;
            squareContents[toSquareArrayPos] = piece;
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
            squareContents[squareArrayPos] = NormalPiece.Empty;
        }

        public PieceTotals CalculatePieceTotals()
        {
            int blackTotal = 0;
            int whiteTotal = 0;

            for (int i = 0; i < piecePositions.Length; i++)
            {
                int amountOfPiece = PiecePositions[i].Count;
                switch ((NormalPiece)i) //switch to using ToValue and ToColor extensions maybe
                {
                    case NormalPiece.BlackPawn:
                        blackTotal += (int)NormalPieceValue.Pawn * amountOfPiece;
                        break;
                    case NormalPiece.WhitePawn:
                        whiteTotal += (int)NormalPieceValue.Pawn * amountOfPiece;
                        break;

                    case NormalPiece.BlackKnight:
                        blackTotal += (int)NormalPieceValue.Knight * amountOfPiece;
                        break;
                    case NormalPiece.WhiteKnight:
                        whiteTotal += (int)NormalPieceValue.Knight * amountOfPiece;
                        break;

                    case NormalPiece.BlackBishop:
                        blackTotal += (int)NormalPieceValue.Bishop * amountOfPiece;
                        break;
                    case NormalPiece.WhiteBishop:
                        whiteTotal += (int)NormalPieceValue.Bishop * amountOfPiece;
                        break;

                    case NormalPiece.BlackRook:
                        blackTotal += (int)NormalPieceValue.Rook * amountOfPiece;
                        break;
                    case NormalPiece.WhiteRook:
                        whiteTotal += (int)NormalPieceValue.Rook * amountOfPiece;
                        break;

                    case NormalPiece.BlackQueen:
                        blackTotal += (int)NormalPieceValue.Queen * amountOfPiece;
                        break;
                    case NormalPiece.WhiteQueen:
                        whiteTotal += (int)NormalPieceValue.Queen * amountOfPiece;
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
                squareContents[i] = NormalPiece.Empty;
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