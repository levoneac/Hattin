using System.Collections.ObjectModel;
using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;
using Hattin.Utils;

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
        private List<AttackInformation> attackInformation; //switch to array
        private NormalPiece[] squareContents;
        private List<AttackProjection>[] attackingSquares; //an array of length 64 that contains lists of sqaures attacked from it
        private List<AttackProjection>[] attackedFrom; //an array of lenght 64 that contains lists of where the source of the attack comes from
        public bool AttackSquaresInitialized { get; private set; }


        public List<BitBoard>[] PiecePositionsBitBoard { get; set; }
        public int NumPieces { get; private set; }
        public PieceList()
        {
            NumPieces = Enum.GetNames(typeof(NormalPiece)).Length;
            piecePositions = new List<BoardSquare>[NumPieces];
            PiecePositionsBitBoard = new List<BitBoard>[NumPieces];
            captureAndBlockingSquares = new List<SideToMove>(64);
            attackInformation = new List<AttackInformation>(64);
            squareContents = new NormalPiece[64];
            attackingSquares = new List<AttackProjection>[64];
            attackedFrom = new List<AttackProjection>[64];
            AttackSquaresInitialized = false;
            for (int i = 0; i < NumPieces; i++)
            {
                piecePositions[i] = new List<BoardSquare>();
                //PiecePositionsBitBoard[i] = new List<BitBoard>();
            }

            for (int i = 0; i < 64; i++)
            {
                captureAndBlockingSquares.Add(SideToMove.None);
                attackInformation.Add(new AttackInformation { AttackTotals = new ColorCount(), Data = new List<AttackProjection>() });
                squareContents[i] = NormalPiece.Empty;
                attackingSquares[i] = new List<AttackProjection>();
                attackedFrom[i] = new List<AttackProjection>();
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
            return attackInformation[arrayPos];
        }

        public NormalPiece GetPieceOnSquare(BoardSquare square)
        {
            int arrayPos = square.ToBase64Int();
            return squareContents[arrayPos];
        }

        public List<AttackProjection> GetAttackedSquaresFromSquare(BoardSquare square)
        {
            int arrayPos = square.ToBase64Int();
            return attackingSquares[arrayPos];
        }

        public List<AttackProjection> GetAttackSourceFromSquare(BoardSquare square)
        {
            int arrayPos = square.ToBase64Int();
            return attackedFrom[arrayPos];
        }

        //Make inremental later
        public void UpdateAllAttackSquares(List<List<AttackProjection>> attackProjections)
        {
            FlushAttackInformation();
            AttackInformation curItem;
            foreach (List<AttackProjection> moveSequence in attackProjections)
            {
                BoardSquare sourceSquare = moveSequence[0].Square;
                foreach (AttackProjection attack in moveSequence)
                {
                    curItem = attackInformation[attack.Square.ToBase64Int()];
                    if (attack.Interaction != SquareInteraction.OwnSquare)
                    {
                        if (attack.XRayLevel == 0)
                        {
                            curItem.AttackTotals.IncrementColor(attack.AsSide);
                        }

                        attackedFrom[attack.Square.ToBase64Int()].Add(moveSequence[0]);
                        attackingSquares[sourceSquare.ToBase64Int()].Add(attack);
                    }

                    curItem.Data.Add(attack);
                }
            }
            AttackSquaresInitialized = true;
        }
        public List<BoardSquare> GetCheckSource(SideToMove sideToMove)
        {
            NormalPiece king = sideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            List<AttackProjection> attackSources = attackedFrom[PiecePositions[(int)king][0].ToBase64Int()];
            return attackSources.Where(i => i.XRayLevel == 0 && i.AsPiece.ToColor() != sideToMove).Select(i => i.Square).ToList();
        }

        public List<BoardSquare> GetAttackSource(BoardSquare attackedSquare, int maxXRayLevel = 0)
        {
            List<AttackProjection> attackSources = attackedFrom[attackedSquare.ToBase64Int()];
            return attackSources.Where(i => i.XRayLevel == maxXRayLevel).Select(i => i.Square).ToList();
        }

        //Can be used for discovery attack search maybe?
        public List<Pin> GetPinnedPieces(NormalPiece[] pinnedAgainst)
        {
            List<Pin> pinnedSquares = new List<Pin>();

            foreach (NormalPiece pinnedToPieceType in pinnedAgainst)
            {
                foreach (BoardSquare pinnedToPiece in PiecePositions[(int)pinnedToPieceType])
                {
                    foreach (AttackProjection source in attackedFrom[pinnedToPiece.ToBase64Int()])
                    {
                        if (NormalPieceClassifications.GetMovementfuncFromPiece(source.AsPiece) != NormalPieceClassifications.SlidingPieces) { continue; }
                        if (pinnedToPieceType.ToColor() == source.AsPiece.ToColor()) { continue; } //discovery possible
                        List<BoardSquare> possiblePinSquares = SquareRange.GetSquaresBetween(pinnedToPiece, source.Square, true);
                        foreach (AttackProjection attackedSquare in attackingSquares[source.Square.ToBase64Int()])
                        {
                            if (possiblePinSquares.Contains(attackedSquare.Square) && attackedSquare.XRayLevel == 0 && attackedSquare.Interaction == SquareInteraction.Attacking)
                            {
                                if (attackedSquare.PieceOnSquare.ToValue() == NormalPieceValue.King) { continue; }
                                //checks if there are any other squares with a piece inbetween the possible pin and the pinnedToPiece
                                if (SquareRange.GetSquaresBetween(attackedSquare.Square, pinnedToPiece, false).Any(i => squareContents[i.ToBase64Int()] != NormalPiece.Empty)) { continue; }

                                pinnedSquares.Add(new Pin(source.Square, source.AsPiece, attackedSquare.Square, attackedSquare.PieceOnSquare, pinnedToPiece, pinnedToPieceType,
                                    pinnedToPieceType.ToValue() == NormalPieceValue.King, SquareRange.GetSquaresBetween(attackedSquare.Square, source.Square, true)));

                                continue;
                            }

                        }
                    }
                }
            }
            return pinnedSquares;
        }

        private void FlushAttackInformation()
        {
            for (int i = 0; i < 64; i++)
            {
                attackInformation[i].Data.Clear();
                attackInformation[i].AttackTotals.Black = 0;
                attackInformation[i].AttackTotals.White = 0;
                attackedFrom[i].Clear();
                attackingSquares[i].Clear();
            }
            AttackSquaresInitialized = false;
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
        public void MovePiece(Move move)
        {
            if (squareContents[move.DestSquare.ToBase64Int()].ToValue() == NormalPieceValue.King)
            {
                throw new ArgumentException($"King on {move.DestSquare} cannot be captured", nameof(move.DestSquare));
            }

            //Check if the piece actually exists
            int indexOfFromSquare = piecePositions[(int)move.Piece].IndexOf(move.FromSquare); //LINQ should be side effect free, so you cant change inplace afaik
            if (indexOfFromSquare == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(move.Piece), $"There is no {move.Piece} on square {move.FromSquare} (moving to {move.DestSquare})");
            }

            //If promotion move
            if (move.PromoteTo != NormalPiece.Empty)
            {
                RemovePiece(move.Piece, move.FromSquare);
                piecePositions[(int)move.PromoteTo].Add(move.DestSquare);
            }
            //If not then move the piece as normal
            else
            {
                piecePositions[(int)move.Piece][indexOfFromSquare] = move.DestSquare;
            }


            int fromSquareArrayPos = move.FromSquare.ToBase64Int();
            int toSquareArrayPos = move.DestSquare.ToBase64Int();


            if (squareContents[toSquareArrayPos] != NormalPiece.Empty)
            {
                RemovePiece(squareContents[toSquareArrayPos], move.DestSquare);
            }
            //There is never anything on an enpassantsquare
            else if (move.EnPassantCaptureSquare != BoardSquare.NoSquare)
            {
                RemovePiece(squareContents[move.EnPassantCaptureSquare.ToBase64Int()], move.EnPassantCaptureSquare);
            }

            captureAndBlockingSquares[fromSquareArrayPos] = SideToMove.None;
            captureAndBlockingSquares[toSquareArrayPos] = move.Piece.ToColor();

            squareContents[fromSquareArrayPos] = NormalPiece.Empty;
            squareContents[toSquareArrayPos] = move.PromoteTo == NormalPiece.Empty ? move.Piece : move.PromoteTo;
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
            FlushAttackInformation();
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