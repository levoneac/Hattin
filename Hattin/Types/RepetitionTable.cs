namespace Hattin.Types
{
    public class RepetitionTable
    {
        private Stack<int> HashStack { get; set; }
        private Dictionary<int, int> PositionHashes { get; set; }

        public RepetitionTable()
        {
            HashStack = new Stack<int>();
            PositionHashes = new Dictionary<int, int>();
        }
        public void PushPosition(int zobristHash)
        {
            HashStack.Push(zobristHash);
            if (ProbePosition(zobristHash))
            {
                PositionHashes[zobristHash] += 1;
            }
            else
            {
                PositionHashes[zobristHash] = 1;
            }
        }

        public void PopPosition()
        {
            int popped = HashStack.Pop();
            int amountOccured = PositionHashes[popped];
            if (amountOccured == 1)
            {
                PositionHashes.Remove(popped);
            }
            else
            {
                PositionHashes[popped] -= 1;
            }
        }

        public bool ProbePosition(int zobristHash)
        {
            if (PositionHashes.TryGetValue(zobristHash, out int exists))
            {
                return true;
            }
            return false;
        }
    }
}