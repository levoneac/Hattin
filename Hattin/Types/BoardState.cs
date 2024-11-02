using System.Text;

namespace Hattin.Types
{
    public class BoardState
    {
        public static readonly SquareIndexType squareIndexing = SquareIndexType.Base_120;
        public static readonly string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private NormalPiece[] board;
        public NormalPiece[] Board
        {
            get { return board; }
            private set { board = value; }
        }

        private Move lastestMove;
        public Move LastestMove
        {
            get { return lastestMove; }
            private set { lastestMove = value; }
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

        private SideToMove sideToMove;
        public SideToMove SideToMove
        {
            get { return sideToMove; }
            private set { sideToMove = value; }
        }

        private BoardSquare enPassantSquare;
        public BoardSquare EnPassantSquare
        {
            get { return enPassantSquare; }
            private set { enPassantSquare = value; }
        }

        private CastleRights castleRights;
        public CastleRights CastleRights
        {
            get { return castleRights; }
            private set { castleRights = value; }
        }

        public BoardState()
        {
            Board = new NormalPiece[(int)squareIndexing];
            LastestMove = new Move();
            PlyCounter = 0;
            PliesWithoutCapture = 0;
            SideToMove = SideToMove.White;
            EnPassantSquare = BoardSquare.NoSquare;
            CastleRights = CastleRights.WhiteKingsideCastle | CastleRights.WhiteQueensideCastle | CastleRights.BlackKingsideCastle | CastleRights.BlackQueensideCastle;

            ProcessFEN(startingFEN);
        }

        public int GetPositionHash()
        {
            return HashCode.Combine(Board, EnPassantSquare, CastleRights, SideToMove);
        }

        public void FlushBoard()
        {

            for (int i = 0; i < Board.Length; i++)
            {
                Board[i] = NormalPiece.Empty;
            }
            PlyCounter = 0;
            PliesWithoutCapture = 0;
            SideToMove = SideToMove.White;
            EnPassantSquare = BoardSquare.NoSquare;
            CastleRights = 0;
        }
        public void PrintBoard(SideToMove perspective, bool moreInfo = false)
        {

            if (perspective == SideToMove.White)
            {
                for (int i = 63; i >= 0; i--)
                {
                    if ((i + 1) % 8 == 0)
                    {
                        Console.WriteLine();
                    }
                    Console.Write($"|{(FENSymbols)Board[Conversions.SquareConversions.Array64To120[i]]}|");
                }
            }
            else
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
            Console.WriteLine();
            if (moreInfo)
            {
                Console.WriteLine($"Total plies: {PlyCounter}");
                Console.WriteLine($"Plies without capture or pawnmove: {PliesWithoutCapture}");
                Console.WriteLine($"Side to move: {SideToMove}");
                Console.WriteLine($"Enpassant squre: {EnPassantSquare}");
                Console.WriteLine($"Castle rights: {CastleRights}");
            }

        }
        public void ProcessFEN(string FEN)
        {
            FlushBoard();
            //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            string[] byRank = FEN.Split(" "); //[board state(0), player to move(1), castle rights(2), enpassant square(3) 50 move rule (in ply)(4), total moves (fullmove)(5)]
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

                    if (Enum.TryParse(typeof(FENSymbols), elem.ToString(), false, out object piece))
                    {
                        Board[Conversions.SquareConversions.Array64To120[boardPointer]] = (NormalPiece)piece;
                        boardPointer--;
                    }
                    else
                    {
                        throw new ArgumentException($"Character: {elem} is not representing a valid piece", nameof(FEN));
                    }


                }
                else
                {
                    throw new ArgumentException($"{elem} is not a letter or number", nameof(FEN));
                }
            }

            //player to move
            if (byRank[1] == "w")
            {
                SideToMove = SideToMove.White;
            }

            else if (byRank[1] == "b")
            {
                SideToMove = SideToMove.Black;
            }
            else
            {
                throw new ArgumentException($"Player to move value of {byRank[1]} is not valid", nameof(FEN));
            }

            //castle rights KQkq
            if (byRank[2][0] != '-')
            {
                foreach (char elem in byRank[2])
                {
                    if (elem == 'K')
                    {
                        CastleRights |= CastleRights.WhiteKingsideCastle;
                    }
                    else if (elem == 'Q')
                    {
                        CastleRights |= CastleRights.WhiteQueensideCastle;
                    }
                    else if (elem == 'k')
                    {
                        CastleRights |= CastleRights.BlackKingsideCastle;
                    }
                    else if (elem == 'q')
                    {
                        CastleRights |= CastleRights.BlackQueensideCastle;
                    }
                    else
                    {
                        throw new ArgumentException($"Castling rights value of {elem} is not valid", nameof(FEN));
                    }
                }
            }


            //enpassant square
            if (byRank[3] == "-")
            {
                EnPassantSquare = BoardSquare.NoSquare;
            }
            else
            {
                if (Enum.TryParse(typeof(BoardSquare), byRank[3], true, out object square))
                {
                    EnPassantSquare = (BoardSquare)square;
                }
                else
                {
                    throw new ArgumentException($"En passantsquare value of {byRank[3]} is not valid", nameof(FEN));
                }
            }

            //50 move rule (in ply)
            if (int.TryParse(byRank[4], out int plies))
            {
                pliesWithoutCapture = plies;
            }
            else
            {
                throw new ArgumentException($"Plies without capture value of {byRank[4]} is not valid", nameof(FEN));
            }

            //total moves (fullmove)
            if (int.TryParse(byRank[5], out int moves))
            {
                if (SideToMove == SideToMove.White)
                {
                    PlyCounter = (moves - 1) * 2;
                }
                else
                {
                    PlyCounter = ((moves - 1) * 2) + 1;
                }
            }
            else
            {
                throw new ArgumentException($"Plies without capture value of {byRank[5]} is not valid", nameof(FEN));
            }
        }


        //boardhash for 3 fold repetition


    }
}