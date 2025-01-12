using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.PositionEvaluators
{
    public class BasicPositionEvaluator : IPositionEvaluator
    {
        public float EvaluateCurrentPosition(BoardState currentBoard)
        {
            PieceTotals totals = currentBoard.PieceProperties.CalculatePieceTotals();
            return totals.White - totals.Black;
        }

        public float EvaluatePositionAfterMove(Move newMove, BoardState currentBoard)
        {
            throw new NotImplementedException();
        }
    }
}