namespace Hattin.Types
{
    public record MoveEvaluation
    {
        public GeneratedMove? Move { get; set; }
        public int Evaluation { get; set; }

        public MoveEvaluation(SideToMove player)
        {
            if (player == SideToMove.Black) { Evaluation = int.MaxValue; }
            else if (player == SideToMove.White) { Evaluation = int.MinValue; }
            Move = null;
        }
        public MoveEvaluation(GeneratedMove move, int evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }

        public void SetToNewMove(GeneratedMove? move, int evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }


    }
}