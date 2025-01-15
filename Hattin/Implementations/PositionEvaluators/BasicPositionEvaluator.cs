using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.PositionEvaluators
{
    public class BasicPositionEvaluator : IPositionEvaluator
    {
        public float EvaluateCurrentPosition(BoardState currentBoard)
        {
            PieceTotals totals = currentBoard.PieceProperties.CalculatePieceTotals();
            int materialScore = totals.White - totals.Black;
            int piecePositionScore = 0;
            foreach (NormalPiece piece in Enum.GetValues(typeof(NormalPiece)))
            {
                foreach (BoardSquare square in currentBoard.PieceProperties.PiecePositions[(int)piece])
                {
                    if (piece.ToColor() == SideToMove.White)
                    {
                        piecePositionScore += NormalPieceSquareScores.GetPieceSquareScore(piece)[square.ToBase64Int()];

                    }
                    else
                    {
                        piecePositionScore -= NormalPieceSquareScores.GetPieceSquareScore(piece)[square.ToBase64Int()];
                    }
                }
            }
            return materialScore + piecePositionScore;
        }

        //well just the move
        public float EvaluatePositionAfterMove(Move newMove, BoardState currentBoard)
        {
            PieceTotals totals = currentBoard.PieceProperties.CalculatePieceTotals();
            int materialScore = totals.White - totals.Black;

            int piecePositionScore = NormalPieceSquareScores.GetPieceSquareScore(newMove.Piece)[newMove.DestSquare.ToBase64Int()];
            return materialScore + piecePositionScore;
        }
    }
}