namespace HattinEngineLibrary.Types
{
    public class AnalyzedPosition
    {
        public MoveEvaluation? BestMove { get; set; }
        public int PositionHash { get; }
        public CancellationToken StopToken { get; }
        public bool IsDone { get; set; }
        public AnalyzedPosition(int numberOfOptions, int positionHash, CancellationToken cancellationToken)
        {
            //BestMoves = new MoveEvaluation[numberOfOptions];
            PositionHash = positionHash;
            IsDone = false;
            StopToken = cancellationToken;

        }

    }
}