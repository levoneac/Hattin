using Hattin.Extensions.NormalPiece;
using Hattin.Interfaces;
using Hattin.Types;
using Hattin.Utils;

namespace Hattin.Implementations.MoveConstraintBuilders
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
        public void SetStopCheck()
        {
            NormalPiece king = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            BoardSquare kingSquare = Board.PieceProperties.GetPiecePositions(king)?[0] ?? throw new Exception($"The king went missing");
            List<BoardSquare> checkingSources = Board.PieceProperties.GetCheckSource(Board.SideToMove);
            List<BoardSquare> checkAvertingSquares = new List<BoardSquare>();
            checkingSources.ForEach(sq => checkAvertingSquares.AddRange(SquareRange.GetSquaresBetween(sq, kingSquare, true)));

            CurrentCollection.Add(StopCheck);

            bool StopCheck(GeneratedMove move)
            {
                if (checkAvertingSquares.Contains(move.DestSquare) && move.DestSquare != kingSquare)
                {
                    return true;
                }
                else if (move.Piece.ToValue() == NormalPieceValue.King)
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
            foreach (var pin in pins)
            {
                pinnedLookup[pin.PinnedPieceSquare] = pin.AllowedSquares;
            }
            if (pinnedLookup.Count > 0)
            {
                CurrentCollection.Add(IsPinRestricted);
            }

            bool IsPinRestricted(GeneratedMove move)
            {
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