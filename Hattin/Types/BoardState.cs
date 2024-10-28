namespace Hattin.Types
{
    public class BoardState
    {
        public static readonly SquareIndexType squareIndexing = SquareIndexType.Base_120;

        private int[] board = new int[(int)squareIndexing];
        public int[] Board
        {
            get { return board; }
            private set { board = value; }
        }

        private Move? lastestMove;
        public Move? LastestMove
        {
            get { return lastestMove; }
            set { lastestMove = value; }
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

        private CastleRights castleRights;
        public CastleRights CastleRights
        {
            get { return castleRights; }
            set { castleRights = value; }
        }


        public void Test()
        {

        }

        //boardhash for 3 fold repetition


    }
}