using Hattin.Extensions.NormalPiece;
using Hattin.Interfaces;
using Hattin.Types;
using Hattin.Utils;

namespace Hattin.Implementations.MoveConstraintBuilders
{
    public class BasicMoveConstraintBuilder : IMoveConstraintBuilder
    {
        //Probably not very multithreading safe with these inner function. Needs some more investigation when that time comes
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