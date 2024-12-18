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
        private static readonly string printDivider = new string('-', 10);

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

        public static void PrintBitBoard(ulong bitBoard)
        {
            Console.WriteLine();
            Console.WriteLine(printDivider);
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write((bitBoard >> i) & mask);
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
        public void SetBitPieceBase_64(int index)
        {
            int bitOffset = index;
            if (bitOffset < 0 || bitOffset > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(index), bitOffset, "Index is outside the board");
            }
            Board |= 1UL << bitOffset;
        }

        //Sets the bit on the given square to 1.
        public void SetBitPieceBase_120(int index)
        {
            int bitOffset = Utils.Conversions.SquareConversions.Convert120To64(index);

            if (bitOffset < 0 || bitOffset > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(index), bitOffset, "Index is outside the board");
            }
            Board |= 1UL << bitOffset;
        }


        //Needs testing
        public ulong GetLastSetBit()
        {
            ulong boardCopy = Board;
            boardCopy &= boardCopy - 1UL;
            return Board & ~boardCopy;
        }

        public static ulong GetLastSetBit(ulong bitBoard)
        {
            ulong boardCopy = bitBoard;
            boardCopy &= boardCopy - 1UL;
            return bitBoard & ~boardCopy;
        }

        public ulong PopLastSetBit()
        {
            ulong lastBit = GetLastSetBit();
            Board ^= lastBit;
            return Board;
        }


        public static ulong PopLastSetBit(ulong bitBoard)
        {
            ulong lastBit = GetLastSetBit(bitBoard);
            bitBoard ^= lastBit;
            return bitBoard;
        }
        public int LastSetBitIndex()
        {
            ulong lastBit = GetLastSetBit();
            for (int i = 0; i < 64; i++) //Find some bitmagic later
            {
                if ((lastBit >> i) == mask)
                {
                    return i;
                }
            }
            return (int)BoardSquare.NoSquare;
        }



        public static int LastSetBitIndex(ulong bitBoard)
        {
            ulong lastBit = GetLastSetBit(bitBoard);
            for (int i = 0; i < 64; i++) //Find some bitmagic later
            {
                if ((lastBit >> i) == mask)
                {
                    return i;
                }
            }
            return (int)BoardSquare.NoSquare;
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

        public static int CountSetBits(ulong bitBoard)
        {
            int count = 0;
            while (bitBoard > 0)
            {
                //Example:
                //0b_1100 - 1ul = 1011 -> 1100 & 1011 = 1000
                //So this removes the least significant bit for each iteration
                bitBoard &= bitBoard - 1UL;
                count++;
            }
            return count;
        }
    }
}