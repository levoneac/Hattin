using System.Text;

namespace Hattin.Types
{
    public class BoardState
    {
        public static readonly SquareIndexType squareIndexing = SquareIndexType.Base_120;
        public static readonly string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

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

        public BoardState()
        {
            ProcessFEN(startingFEN);
        }

        public void PrintBoard()
        {
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                {
                    Console.WriteLine();
                }
                Console.Write($"|{(FENSymbols)Board[Conversions.SquareConversions.Array64To120[i]]}|");
            }
        }
        public void ProcessFEN(string FEN)
        {
            //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            string[] byRank = FEN.Split(" "); //[board state, player to move, castle rights, 50 move rule (in ply), total moves (fullmove)]
            int boardPointer = 63; //FEN starts from the last square (H8)

            //Board state
            foreach (char elem in byRank[0])
            {
                if (elem == '/')
                {
                    continue;
                }
                if (char.IsNumber(elem))
                {
                    //rank number
                    boardPointer -= (int)char.GetNumericValue(elem);
                }
                else if (char.IsLetter(elem))
                {

                    if (Enum.TryParse(typeof(FENSymbols), elem.ToString(), false, out object val))
                    {
                        Board[Conversions.SquareConversions.Array64To120[boardPointer]] = (int)val;
                        boardPointer--;
                    }
                    else
                    {
                        throw new Exception($"Character: {elem} is not representing a valid piece ");
                    }


                }
                else
                {
                    throw new Exception($"{elem} is not a letter or number");
                }
            }
            //TODOS:
            //player to move

            //castle rights

            //50 move rule (in ply)

            //total moves (fullmove)

        }


        //boardhash for 3 fold repetition


    }
}