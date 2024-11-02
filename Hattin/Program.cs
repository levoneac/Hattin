using Hattin.Types;

namespace Hattin
{
    class Engine
    {
        private BoardState currentBoardState;
        public BoardState CurrentBoardState
        {
            get { return currentBoardState; }
            set { currentBoardState = value; }
        }

        //Does one need to keep a full copy of the board state for every move?
        //Maybe make another datatype which contains a board hash and move
        private List<BoardState> boardHistory;
        public List<BoardState> BoardHistory
        {
            get { return boardHistory; }
            set { boardHistory = value; }
        }


    }

    class Program
    {
        public static void Main(string[] args)
        {
            /*
                //Lists dont copy the item, its still a reference
                //Simple types (primitives?) like ints do get copied and cant be changed from outside
                List<int[]> a = new();
                int[] b = new int[1];
                int x = 21;
                b[0] = x;
                a.Add(b);
                Console.WriteLine(a[0][0]); //21
                x = 21344;
                b[0] = 4124;
                Console.WriteLine(a[0][0]); //4124
            */

            BitBoard bb = new();
            bb.Board = 256 + 512 + 1024 + 2048 + 4096 + 8192 + 16384 + 32768;
            bb.PrintBitBoard();
            //bb.Board |= 1UL << Conversions.SquareConversions.Array120To64[(int)BoardSquares.H8];
            bb.PrintBitBoard();
            bb.AddPieceBase_120(23);
            bb.PrintBitBoard();
            Console.WriteLine(bb.CountSetBits());
            BitBoard.PrintBitBoard(bb.GetLastSetBit());
            Console.WriteLine(bb.LastSetBitIndex());
            bb.PopLastSetBit();
            bb.PrintBitBoard();
            bb.PopLastSetBit();
            bb.PrintBitBoard();
            bb.PopLastSetBit();
            bb.PrintBitBoard();
            bb.PopLastSetBit();
            bb.PrintBitBoard();

            ulong d = (ulong)new Random().NextInt64();
            BoardState board = new();
            board.ProcessFEN("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2 ");
            board.PrintBoard(SideToMove.Black, true);
            Console.WriteLine(board.GetPositionHash());




            /*
                |R||N||B||K||Q||B||N||R|
                |P||P||P||P||P||P||P||P|
                |0||0||0||0||0||0||0||0|
                |0||0||0||0||0||0||0||0|
                |0||0||0||0||0||0||0||0|
                |0||0||0||0||0||0||0||0|
                |p||p||p||p||p||p||p||p|
                |r||n||b||k||q||b||n||r|
            */


            /*
            Console.WriteLine();
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write("|" + Conversions.SquareConversions.Array64To120[i] + "|");
            }

            Console.WriteLine();
            for (int i = 21; i < 99; i++)
            {
                int conv = Conversions.SquareConversions.Array120To64[i];
                if (conv % 8 == 0 && conv != 112)
                {
                    Console.WriteLine();
                }
                Console.Write("|" + conv + "|");
            }
            */

        }
    }
}