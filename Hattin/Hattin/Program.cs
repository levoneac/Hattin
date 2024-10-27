using Hattin.Types;

namespace Hattin
{
    class Engine
    {
        private int[] board = new int[120];
        public int[] Board
        {
            get { return board; }
            private set { board = value; }
        }

        private int plyCounter;
        public int PlyCounter
        {
            get { return plyCounter; }
            private set { plyCounter = value; }
        }

        private int pliesWithoutCapture;
        public int PliesWithoutCapture
        {
            get { return pliesWithoutCapture; }
            private set { pliesWithoutCapture = value; }
        }

        private SideEnum sideToMove;
        public SideEnum SideToMove
        {
            get { return sideToMove; }
            private set { sideToMove = value; }
        }

        private EnPassantSquares enPassantSquares;
        public EnPassantSquares EnPassantSquares
        {
            get { return enPassantSquares; }
            private set { enPassantSquares = value; }
        }



        //boardhash for 3 fold repetition



    }
}