using Microsoft.VisualBasic;

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
        private readonly string printDivider = new string('-', 10);

        public static readonly ulong mask = 1UL;
        public static readonly SquareIndexType squareIndexing = SquareIndexType.Base_64;

        public void PrintBitBoard()
        {
            Console.WriteLine();
            Console.WriteLine(printDivider);
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write((Board >> i) & mask);
            }
            Console.WriteLine();
            Console.Write(printDivider);
        }

        public bool CheckForPiece(int index)
        {
            if (((Board >> index) & mask) > 0)
            {
                return true;
            }
            return false;
        }

        //Sets the bit on the given square to 1.
        public void AddPieceBase_64(int index)
        {
            int bitOffset = index;
            if (bitOffset < 0 || bitOffset > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(index), bitOffset, "Index is outside the board");
            }
            Board |= 1UL << bitOffset;
        }

        //Sets the bit on the given square to 1.
        public void AddPieceBase_120(int index)
        {
            int bitOffset = Conversions.SquareConversions.Convert120To64(index);

            if (bitOffset < 0 || bitOffset > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(index), bitOffset, "Index is outside the board");
            }
            Board |= 1UL << bitOffset;
        }



        public int PopBitBoard()
        {
            return 0;
        }

        public int CountSetBits()
        {
            ulong boardCopy = Board;
            int count = 0;
            while (boardCopy > 0)
            {
                //Example:
                //0b_1100 - 1ul = 1011 -> 1100 & 1011 = 1000
                //So this removes the least significant bit for each iteration
                boardCopy &= boardCopy - 1UL;
                count++;
            }
            return count;
        }
    }
}