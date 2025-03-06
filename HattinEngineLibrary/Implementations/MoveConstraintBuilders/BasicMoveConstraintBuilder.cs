using HattinEngineLibrary.Extensions.NormalPiece;
using HattinEngineLibrary.Extensions.Squares;
using HattinEngineLibrary.Interfaces;
using HattinEngineLibrary.Types;
using HattinEngineLibrary.Utils;

namespace HattinEngineLibrary.Implementations.MoveConstraintBuilders
{
    //Builds a set of functions that filter out moves based on enabled constraints
    public class BasicMoveConstraintBuilder : IMoveConstraintBuilder
    {
        private List<Func<GeneratedMove, bool>> CurrentCollection { get; set; }
        private BoardState Board { get; set; }
        public BasicMoveConstraintBuilder(BoardState board)
        {
            CurrentCollection = new List<Func<GeneratedMove, bool>>();
            Board = board;
        }
        public void Reset()
        {
            CurrentCollection.Clear();
        }

        //C# aparently has closures, so the variables of the outer function are still in scope for the inner function after the return
        //Maybe it would be more efficient to save checkAvertingSquares in a property of this class, but for now it works
        public void SetStopCheck(List<BoardSquare> checkingSources)
        {
            NormalPiece king = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            BoardSquare kingSquare = Board.PieceProperties.GetPiecePositions(king)?[0] ?? throw new Exception($"The king went missing");
            //List<BoardSquare> checkingSources = Board.PieceProperties.GetCheckSource(Board.SideToMove); //Double work, done before setting this constraint
            List<BoardSquare> checkAvertingSquares = new List<BoardSquare>();
            checkingSources.ForEach(sq => checkAvertingSquares.AddRange(SquareRange.GetSquaresBetween(sq, kingSquare, true))); //unnecessary if checkingsources is over 1

            CurrentCollection.Add(StopCheck);

            bool StopCheck(GeneratedMove move)
            {
                //block with a piece if not doublecheck
                if (checkingSources.Count <= 1 && checkAvertingSquares.Contains(move.DestSquare) && move.DestSquare != kingSquare)
                {
                    return true;
                }
                //run away
                else if (move.Piece.ToValue() == NormalPieceValue.King && move.RookCastleFromSquare == BoardSquare.NoSquare && move.RookCastleToSquare == BoardSquare.NoSquare)
                {
                    return true;
                }
                return false;
            }
        }

        public void SetPinRestriction()
        {
            List<Pin> pins = Board.PieceProperties.GetPinnedPieces(NormalPieceClassifications.Kings);
            Dictionary<BoardSquare, List<BoardSquare>> pinnedLookup = new Dictionary<BoardSquare, List<BoardSquare>>();

            //figure out less wasteful way later
            Dictionary<BoardSquare, bool> enPassantLookup = new Dictionary<BoardSquare, bool>();

            foreach (var pin in pins)
            {
                pinnedLookup[pin.PinnedPieceSquare] = pin.AllowedSquares;
                if (pin.EnPassantPin)
                {
                    enPassantLookup[pin.PinnedPieceSquare] = true;
                }
            }
            if (pinnedLookup.Count > 0)
            {
                CurrentCollection.Add(IsNotPinRestricted);
            }


            bool IsNotPinRestricted(GeneratedMove move)
            {
                //check if enpassant-pinned
                if (enPassantLookup.TryGetValue(move.FromSquare, out bool isEnPassantPin))
                {
                    //if capture then dont allow
                    if (move.IsEnPassant)
                    {
                        return false;
                    }
                    //If its not en passant, allow the move as there is another piece to block the attack
                    return true;
                }
                if (pinnedLookup.TryGetValue(move.FromSquare, out List<BoardSquare>? allowedSquares))
                {
                    if (allowedSquares?.Contains(move.DestSquare) ?? false)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        public Func<List<GeneratedMove>, List<GeneratedMove>>? GetConstraintFunction()
        {
            if (CurrentCollection.Count > 0)
            {
                return ConstraintFilter;
            }
            else
            {
                return null;
            }

            List<GeneratedMove> ConstraintFilter(List<GeneratedMove> moves)
            {
                List<GeneratedMove> allowedMoves = new List<GeneratedMove>();
                bool curMoveAllowed;
                foreach (var move in moves)
                {
                    curMoveAllowed = true;
                    foreach (var constraint in CurrentCollection)
                    {
                        if (!constraint.Invoke(move))
                        {
                            curMoveAllowed = false;
                            break;
                        }
                    }
                    if (curMoveAllowed) { allowedMoves.Add(move); }
                }
                return allowedMoves;
            }
        }
    }
}