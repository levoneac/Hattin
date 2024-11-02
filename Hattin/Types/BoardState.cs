using System.Collections.ObjectModel;
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

        private PieceList pieceProperties; //Array of lenght of NormalPiece enum, with each index refering to a list of squares those pieces occupy
        public PieceList PieceProperties
        {
            get { return pieceProperties; }
            private set { pieceProperties = value; }
        }

        private Move lastestMove;
        public Move LastestMove
        {
            get { return lastestMove; }
            private set { lastestMove = value; }
        }

        private List<Move> moveHistory;


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
            PieceProperties = new PieceList();
            moveHistory = new List<Move>();
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

        public ReadOnlyCollection<Move> GetMoveHistory()
        {
            return moveHistory.AsReadOnly();
        }

        public void UpdatePieceBalance()
        {

        }

        public void FlushBoard()
        {

            for (int i = 0; i < Board.Length; i++)
            {
                Board[i] = NormalPiece.Empty;
            }
            PieceProperties.ClearPieceList();
            PlyCounter = 0;
            PliesWithoutCapture = 0;
            SideToMove = SideToMove.White;
            EnPassantSquare = BoardSquare.NoSquare;
            CastleRights = 0;
        }
        public void PrintBoard(SideToMove perspective, bool moreInfo = false)
        {
            Console.WriteLine();
            if (perspective == SideToMove.White)
            {
                for (int i = 56; i >= 0; i++)
                {
                    Console.Write($" {(FENSymbols)Board[Conversions.SquareConversions.Array64To120[i]]} ");
                    if ((i + 1) % 8 == 0)
                    {
                        Console.WriteLine();
                        i -= 16;
                    }
                }
            }
            else
            {
                for (int i = 7; i < 64; i--)
                {
                    Console.Write($" {(FENSymbols)Board[Conversions.SquareConversions.Array64To120[i]]} ");
                    if (i % 8 == 0)
                    {
                        Console.WriteLine();
                        i += 16;
                    }
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
            string[] FENparts = FEN.Split(" "); //[board state(0), player to move(1), castle rights(2), enpassant square(3) 50 move rule (in ply)(4), total moves (fullmove)(5)]
            int boardPointer = 56; //FEN starts from square (A8)

            //Board state
            bool changeRankNextIter = false; //true when a rank has been exhausted
            int elemsInRank = 0; //tracks that all the 8 squares in a rank is described
            int currentRank = 8;
            foreach (char elem in FENparts[0])
            {
                if (elem == '/')
                {
                    if (elemsInRank != 8)
                    {
                        throw new ArgumentException($"FEN not valid, elems in rank {currentRank} is {elemsInRank} rather than 8", nameof(FEN));
                    }
                    elemsInRank = 0;
                    currentRank--;
                    continue;
                }

                if (char.IsNumber(elem))
                {
                    //rank number
                    int spaces = (int)char.GetNumericValue(elem);
                    boardPointer += spaces;
                    elemsInRank += spaces;
                    if (boardPointer % 8 == 0)
                    {
                        changeRankNextIter = true;
                    }
                }
                else if (char.IsLetter(elem))
                {

                    if (Enum.TryParse(typeof(FENSymbols), elem.ToString(), false, out object piece))
                    {
                        int realPosition = Conversions.SquareConversions.Array64To120[boardPointer];
                        Board[realPosition] = (NormalPiece)piece;
                        PieceProperties.PiecePositions[(int)piece].Add((BoardSquare)realPosition);
                        Console.WriteLine($"{(BoardSquare)realPosition}: {(NormalPiece)piece}, ({(int)boardPointer})");
                        boardPointer++;
                        elemsInRank++;
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

                //logic to jump back 2 rows when current row is done
                if (changeRankNextIter)
                {
                    boardPointer -= 16;
                    changeRankNextIter = false;
                }
                else if ((boardPointer + 1) % 8 == 0)
                {
                    changeRankNextIter = true;
                }

                //Console.WriteLine("{0}, {1}: ", boardPointer, changeRankNextIter);
            }

            //player to move
            if (FENparts[1] == "w")
            {
                SideToMove = SideToMove.White;
            }

            else if (FENparts[1] == "b")
            {
                SideToMove = SideToMove.Black;
            }
            else
            {
                throw new ArgumentException($"Player to move value of {FENparts[1]} is not valid", nameof(FEN));
            }

            //castle rights KQkq
            if (FENparts[2][0] != '-')
            {
                foreach (char elem in FENparts[2])
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
            if (FENparts[3] == "-")
            {
                EnPassantSquare = BoardSquare.NoSquare;
            }
            else
            {
                if (Enum.TryParse(typeof(BoardSquare), FENparts[3], true, out object square))
                {
                    EnPassantSquare = (BoardSquare)square;
                }
                else
                {
                    throw new ArgumentException($"En passantsquare value of {FENparts[3]} is not valid", nameof(FEN));
                }
            }

            //50 move rule (in ply)
            if (int.TryParse(FENparts[4], out int plies))
            {
                pliesWithoutCapture = plies;
            }
            else
            {
                throw new ArgumentException($"Plies without capture value of {FENparts[4]} is not valid", nameof(FEN));
            }

            //total moves (fullmove)
            if (int.TryParse(FENparts[5], out int moves))
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
                throw new ArgumentException($"Plies without capture value of {FENparts[5]} is not valid", nameof(FEN));
            }
        }


        //boardhash for 3 fold repetition


    }
}