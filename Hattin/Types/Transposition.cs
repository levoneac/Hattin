namespace Hattin.Types
{
    public class Transposition
    {
        public GeneratedMove Move { get; set; }
        public float Evaluation { get; set; }
        public int Depth { get; set; }
        public TranspositionEntryType Type { get; set; }

        public Transposition(MoveEvaluation move, int depth, TranspositionEntryType type)
        {
            Move = move.Move;
            Evaluation = move.Evaluation;
            Depth = depth;
            Type = type;
        }
    }
}