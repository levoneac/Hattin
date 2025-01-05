namespace Hattin.Types
{
    public class MoveEvaluation
    {
        public Move Move { get; }
        public float Evaluation { get; }
        public MoveEvaluation(Move move, float evaluation)
        {
            Move = move;
            Evaluation = evaluation;
        }


    }
}