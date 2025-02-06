namespace Hattin.Types
{
    public record MoveEvaluation
    {
        public GeneratedMove? Move { get; set; }
        public int Evaluation { get; set; }
        public List<GeneratedMove> PV { get; set; }

        public MoveEvaluation(SideToMove player)
        {
            if (player == SideToMove.Black) { Evaluation = int.MaxValue; }
            else if (player == SideToMove.White) { Evaluation = int.MinValue; }
            Move = null;
            PV = [];
        }
        public MoveEvaluation(GeneratedMove? move, int evaluation)
        {
            Move = move;
            Evaluation = evaluation;
            PV = [move];
        }

        public void SetToNewMove(GeneratedMove? move, int evaluation, List<GeneratedMove> pV)
        {
            Move = move;
            Evaluation = evaluation;
            PV = pV;
        }


    }
}