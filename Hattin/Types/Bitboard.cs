namespace Hattin.Types
{
    public class BitBoard
    {
        private ulong board;
        public ulong Board
        {
            get { return board; }
            set { board = value; }
        }

        public static readonly ulong mask = 1UL;
        public static readonly SquareIndexType squareIndexing = SquareIndexType.Base_64;

        public void PrintBitBoard()
        {
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write((Board >> i) & mask);
            }
        }
    }
}