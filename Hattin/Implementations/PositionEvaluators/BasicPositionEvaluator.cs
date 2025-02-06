using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.PositionEvaluators
{
    public class BasicPositionEvaluator : IPositionEvaluator
    {
        public int EvaluateCurrentPosition(BoardState currentBoard)
        {
            PieceList board = currentBoard.PieceProperties;
            PieceTotals totals = board.CalculatePieceTotals();
            int materialScore = totals.White - totals.Black;
            int piecePositionScore = 0;
            int mobilityScore = 0;
            foreach (NormalPiece piece in Enum.GetValues(typeof(NormalPiece)))
            {
                foreach (BoardSquare square in board.PiecePositions[(int)piece])
                {
                    if (piece.ToColor() == SideToMove.White)
                    {
                        piecePositionScore += NormalPieceSquareScores.GetPieceSquareScore(piece)[square.ToBase64Int()];
                        mobilityScore += (board.GetAttackedSquaresFromSquare(square).Count - board.GetAttackSourceFromSquare(square).Count) * 2;
                    }
                    else
                    {
                        piecePositionScore -= NormalPieceSquareScores.GetPieceSquareScore(piece)[square.ToBase64Int()];
                        mobilityScore -= (board.GetAttackedSquaresFromSquare(square).Count - board.GetAttackSourceFromSquare(square).Count) * 2;
                    }
                }
            }
            return materialScore + piecePositionScore + mobilityScore;
        }

        //well just the move
        public int EvaluatePositionAfterMove(Move newMove, BoardState currentBoard)
        {
            PieceTotals totals = currentBoard.PieceProperties.CalculatePieceTotals();
            int materialScore = totals.White - totals.Black;

            int piecePositionScore = NormalPieceSquareScores.GetPieceSquareScore(newMove.Piece)[newMove.DestSquare.ToBase64Int()];
            return materialScore + piecePositionScore;
        }
    }
}