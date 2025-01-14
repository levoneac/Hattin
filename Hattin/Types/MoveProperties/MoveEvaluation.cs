namespace Hattin.Types
{
    public record MoveEvaluation
    {
        public GeneratedMove Move { get; set; }
        public float Evaluation { get; set; }
        public MoveEvaluation(GeneratedMove move, float evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }


    }
}