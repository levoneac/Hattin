﻿using Hattin.Types;

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
            Console.WriteLine();
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write("|" + Conversions.SquareConversions.Convert(i, SquareIndexType.Base_64, SquareIndexType.Base_120) + "|");
            }

            Console.WriteLine();
            for (int i = 21; i < 99; i++)
            {
                int conv = Conversions.SquareConversions.Convert(i, SquareIndexType.Base_120, SquareIndexType.Base_64);
                if (conv % 8 == 0 && conv != 112)
                {
                    Console.WriteLine();
                }
                Console.Write("|" + conv + "|");
            }
        }
    }
}