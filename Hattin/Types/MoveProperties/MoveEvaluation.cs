namespace Hattin.Types
{
    public class MoveEvaluation
    {
        public GeneratedMove Move { get; }
        public float Evaluation { get; }
        public MoveEvaluation(GeneratedMove move, float evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }


    }
}