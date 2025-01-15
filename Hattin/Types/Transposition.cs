namespace Hattin.Types
{
    public class Transposition
    {
        public GeneratedMove Move { get; set; }
        public float Evaluation { get; set; }
        public int Depth { get; set; }

        public Transposition(GeneratedMove move, float evaluation, int depth)
        {
            Move = move;
            Evaluation = evaluation;
            Depth = depth;

        }
    }
}