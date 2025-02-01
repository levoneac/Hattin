namespace Hattin.Types
{
    public record MoveEvaluation
    {
        public GeneratedMove? Move { get; set; }
        public float Evaluation { get; set; }

        public MoveEvaluation(SideToMove player)
        {
            if (player == SideToMove.Black) { Evaluation = float.MaxValue; }
            else if (player == SideToMove.White) { Evaluation = float.MinValue; }
            Move = null;
        }
        public MoveEvaluation(GeneratedMove move, float evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }

        public void SetToNewMove(GeneratedMove? move, float evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }


    }
}