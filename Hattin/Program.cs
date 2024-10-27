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
        }
    }
}