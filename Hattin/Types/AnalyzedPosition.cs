namespace Hattin.Types
{
    public class AnalyzedPosition
    {
        public MoveEvaluation[] BestMoves { get; set; }
        public int PositionHash { get; set; }
        public AnalyzedPosition(int numberOfOptions)
        {
            BestMoves = new MoveEvaluation[numberOfOptions];

        }

    }
}