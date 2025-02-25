using System.Collections.ObjectModel;
using Hattin.Events.EventArguments;
using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;
using Hattin.Utils.Conversions;

namespace Hattin.Types
{
    public class BoardState : IBoard
    {
        public static readonly SquareIndexType squareIndexing = SquareIndexType.Base_120;
        public const string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public event EventHandler<NewMoveEventArgs> NewMoveEvent;
        public NormalPiece[] Board { get; set; }
        public PieceList PieceProperties { get; set; }
        public Move LastestMove { get; set; }
        private Stack<PlayedMove> moveHistory;
        public int PlyCounter { get; set; }
        public int PliesWithoutCapture { get; set; }
        public SideToMove SideToMove { get; set; }
        public BoardSquare EnPassantSquare { get; set; }
        public CastleRights CastleRights { get; set; }
        public bool IsCheck { get; set; }
        public ZobristHash PositionHash { get; set; }
        public GameResult GameResult { get; set; }
        public RepetitionTable RepetitionTable { get; set; }
        public NeuralNetRepresentation NeuralNetRepresentation { get; set; }

        public BoardState()
        {
            Board = new NormalPiece[(int)squareIndexing];
            PieceProperties = new PieceList();
            LastestMove = new Move();
            moveHistory = new Stack<PlayedMove>();
            PlyCounter = 0;
            PliesWithoutCapture = 0;
            SideToMove = SideToMove.White;
            EnPassantSquare = BoardSquare.NoSquare;
            CastleRights = CastleRights.WhiteKingsideCastle | CastleRights.WhiteQueensideCastle | CastleRights.BlackKingsideCastle | CastleRights.BlackQueensideCastle;
            IsCheck = false;
            PositionHash = new ZobristHash();
            RepetitionTable = new RepetitionTable();
            NeuralNetRepresentation = new NeuralNetRepresentation();
            //PositionHashes = new Dictionary<int, int>();

            //NewMoveEvent += PrintMove;
            //NewMoveEvent += UpdatePositionHashes;
            ProcessFEN(startingFEN);
        }


        //private void UpdatePositionHashes()
        //{
        //    int currentPositionHash = GetPositionHash();
        //    if (PositionHashes.TryGetValue(currentPositionHash, out int current))
        //    {
        //        PositionHashes[currentPositionHash] = current + 1;
        //        if (PositionHashes[currentPositionHash] >= 3)
        //        {
        //            GameResult = GameResult.Draw;
        //        }
        //    }
        //    else
        //    {
        //        PositionHashes.Add(currentPositionHash, 1);
        //    }
        //}
        public virtual void OnNewMoveEvent(NewMoveEventArgs eventArgs)
        {
            NewMoveEvent?.Invoke(this, eventArgs);
        }

        public void PrintMove(object? sender, NewMoveEventArgs eventArgs)
        {
            Console.WriteLine($"Move {eventArgs.Piece} from {eventArgs.FromSquare} to {eventArgs.DestSquare}");
        }

        public void UpdateCastleRights(Move move)
        {
            BoardSquare pieceSquare = move.FromSquare;
            BoardSquare captureSquare = move.DestSquare;
            if (pieceSquare == BoardSquare.E1)
            {
                CastleRights &= ~(CastleRights.WhiteKingsideCastle | CastleRights.WhiteQueensideCastle);
            }
            else if (pieceSquare == BoardSquare.E8)
            {
                CastleRights &= ~(CastleRights.BlackKingsideCastle | CastleRights.BlackQueensideCastle);
            }
            if (pieceSquare == BoardSquare.A1 || captureSquare == BoardSquare.A1)
            {
                CastleRights &= ~CastleRights.WhiteQueensideCastle;
            }
            if (pieceSquare == BoardSquare.H1 || captureSquare == BoardSquare.H1)
            {
                CastleRights &= ~CastleRights.WhiteKingsideCastle;
            }
            if (pieceSquare == BoardSquare.A8 || captureSquare == BoardSquare.A8)
            {
                CastleRights &= ~CastleRights.BlackQueensideCastle;
            }
            if (pieceSquare == BoardSquare.H8 || captureSquare == BoardSquare.H8)
            {
                CastleRights &= ~CastleRights.BlackKingsideCastle;
            }
        }

        public void MovePiece(Move move, bool manualRepetitionTable = false)
        {
            NormalPiece pieceAfterMove = move.PromoteTo == NormalPiece.Empty ? move.Piece : move.PromoteTo;
            //120 based array for some reason
            Board[(int)move.FromSquare] = NormalPiece.Empty;
            Board[(int)move.DestSquare] = pieceAfterMove;

            NeuralNetRepresentation.SetValue(move.Piece, move.FromSquare, false);
            NeuralNetRepresentation.SetValue(pieceAfterMove, move.DestSquare, true);

            //Castle move 
            if (move.RookCastleToSquare != BoardSquare.NoSquare && move.RookCastleFromSquare != BoardSquare.NoSquare)
            {
                NormalPiece rook = PieceProperties.GetPieceOnSquare(move.RookCastleFromSquare);
                Board[(int)move.RookCastleToSquare] = rook;
                Board[(int)move.RookCastleFromSquare] = NormalPiece.Empty;

                NeuralNetRepresentation.SetValue(rook, move.RookCastleFromSquare, false);
                NeuralNetRepresentation.SetValue(rook, move.RookCastleToSquare, true);
            }

            NormalPiece pieceOnDestSquare = PieceProperties.GetPieceOnSquare(move.DestSquare);

            //Save prev move data and state
            moveHistory.Push(new PlayedMove
            {
                CastleRights = CastleRights,
                EnPassantSquare = EnPassantSquare,
                EnPassantCaptureSquare = move.EnPassantCaptureSquare,
                PlyCounter = PlyCounter,
                PliesWithoutCapture = PliesWithoutCapture,
                SideToMove = SideToMove,
                IsCheck = IsCheck,

                PromotedFromPiece = move.Piece,
                PromotedToPiece = pieceAfterMove,
                FromSquare = move.FromSquare,
                DestSquare = move.DestSquare,
                PieceOnDestSquare = pieceOnDestSquare,
                RookSourceSquare = move.RookCastleFromSquare,
                RookDestSquare = move.RookCastleToSquare,

            });

            if (CastleRights != 0) { UpdateCastleRights(move); }

            //Enpassant square
            EnPassantSquare = move.EnPassantSquare;

            //Enpassant capture
            if (move.EnPassantCaptureSquare != BoardSquare.NoSquare)
            {
                Board[(int)move.EnPassantCaptureSquare] = NormalPiece.Empty;
                NeuralNetRepresentation.SetValue(PieceProperties.GetPieceOnSquare(move.EnPassantCaptureSquare), move.EnPassantCaptureSquare, false);
            }

            PlyCounter++;
            if (pieceOnDestSquare != NormalPiece.Empty)
            {
                NeuralNetRepresentation.SetValue(pieceOnDestSquare, move.DestSquare, false);
            }
            if (pieceOnDestSquare != NormalPiece.Empty || move.EnPassantCaptureSquare != BoardSquare.NoSquare || move.Piece.ToValue() != NormalPieceValue.Pawn)
            { PliesWithoutCapture++; }
            else { PliesWithoutCapture = 0; }

            SideToMove = SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;

            PositionHash.MovePiece(move, this);
            if (!manualRepetitionTable)
            {
                RepetitionTable.PushPosition(PositionHash.CurrentPositionHash);
            }

            PieceProperties.MovePiece(move);

            //NewMoveEventArgs eventArgs = new NewMoveEventArgs(move);
            //OnNewMoveEvent(eventArgs);
        }

        public void UndoLastMove(bool manualRepetitionTable = false)
        {
            //ALT. 1: Use FEN to restore the boardstate (loses detailed move history outside the info in the FEN strings)
            //ALT. 2: Restart the board and play through the game again (Pointless, since the board is restarted through FEN anyway)
            //ALT. 3: Undo the last move (Fastest probably, but difficult to get right)
            //ALT. 4: Copy the current state into an array (same as FEN? but uses more space)
            PlayedMove moveToUndo = moveHistory.Pop();
            UndoMove(moveToUndo, manualRepetitionTable);
        }

        public void UndoMove(PlayedMove move, bool manualRepetitionTable = false)
        {

            Board[(int)move.FromSquare] = move.PromotedFromPiece;
            Board[(int)move.DestSquare] = move.PieceOnDestSquare;

            NeuralNetRepresentation.SetValue(move.PromotedToPiece, move.DestSquare, false);
            NeuralNetRepresentation.SetValue(move.PromotedFromPiece, move.FromSquare, true);
            if (move.PieceOnDestSquare != NormalPiece.Empty)
            {
                NeuralNetRepresentation.SetValue(move.PieceOnDestSquare, move.DestSquare, true);
            }

            if (move.RookSourceSquare != BoardSquare.NoSquare && move.RookDestSquare != BoardSquare.NoSquare)
            {
                NormalPiece rook = PieceProperties.GetPieceOnSquare(move.RookDestSquare);
                Board[(int)move.RookSourceSquare] = rook;
                Board[(int)move.RookDestSquare] = NormalPiece.Empty;

                NeuralNetRepresentation.SetValue(rook, move.RookSourceSquare, true);
                NeuralNetRepresentation.SetValue(rook, move.RookDestSquare, false);
            }

            CastleRights = move.CastleRights;
            EnPassantSquare = move.EnPassantSquare;
            if (move.EnPassantCaptureSquare != BoardSquare.NoSquare)
            {
                NormalPiece pawn = move.SideToMove == SideToMove.White ? NormalPiece.BlackPawn : NormalPiece.WhitePawn;
                Board[(int)move.EnPassantCaptureSquare] = pawn;
                NeuralNetRepresentation.SetValue(pawn, move.EnPassantCaptureSquare, true);
            }
            IsCheck = move.IsCheck;
            PlyCounter = move.PlyCounter; //strip
            PliesWithoutCapture = move.PliesWithoutCapture;
            SideToMove = move.SideToMove; //strip
            if (!manualRepetitionTable)
            {
                //Should be changed to Remove() to fit with the rest of the function but its not really neccesary
                RepetitionTable.PopPosition();
            }
            PositionHash.UndoMove(move, this);
            PieceProperties.UndoMove(move);
        }

        private void FlushBoard()
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
            moveHistory.Clear();
            RepetitionTable.Clear();
            NeuralNetRepresentation.Clear();
        }
        public void PrintBoard(SideToMove perspective, bool moreInfo = false)
        {
            Console.WriteLine();
            if (perspective == SideToMove.White)
            {
                for (int i = 56; i >= 0; i++)
                {
                    Console.Write($" {(FENSymbols)Board[SquareConversions.Array64To120[i]]} ");
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
                    Console.Write($" {(FENSymbols)Board[SquareConversions.Array64To120[i]]} ");
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

        public void PrintAttackTotals(SideToMove perspective)
        {
            Console.WriteLine();
            ColorCount curCount;
            int curTotal;
            if (perspective == SideToMove.White)
            {
                for (int i = 56; i >= 0; i++)
                {
                    curCount = PieceProperties.GetAttackCountOnSquare((BoardSquare)SquareConversions.Array64To120[i]).AttackTotals;
                    curTotal = curCount.White - curCount.Black;
                    Console.Write($" {(curTotal >= 0 ? $"+{curTotal}" : curTotal)} ");
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
                    curCount = PieceProperties.GetAttackCountOnSquare((BoardSquare)SquareConversions.Array64To120[i]).AttackTotals;
                    curTotal = curCount.White - curCount.Black;
                    Console.Write($" {(curTotal >= 0 ? $"+{curTotal}" : curTotal)} ");
                    if (i % 8 == 0)
                    {
                        Console.WriteLine();
                        i += 16;
                    }
                }
            }
        }

        public void ProcessFEN(string FEN = startingFEN)
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
                        throw new ArgumentException($"FEN not valid, elems on rank {currentRank} is '{elemsInRank}' rather than '8'", nameof(FEN));
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

                    if (Enum.TryParse(typeof(FENSymbols), elem.ToString(), false, out object? piece))
                    {
                        int realPosition = Utils.Conversions.SquareConversions.Array64To120[boardPointer];
                        Board[realPosition] = (NormalPiece)piece;
                        PieceProperties.AddPiece((NormalPiece)piece, (BoardSquare)realPosition);
                        NeuralNetRepresentation.SetValue((NormalPiece)piece, (BoardSquare)realPosition, true);
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
                    if (elem == '-') { continue; }
                    else if (elem == 'K')
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
                if (Enum.TryParse(typeof(BoardSquare), FENparts[3], true, out object? square))
                {
                    EnPassantSquare = (BoardSquare)square;
                }
                else
                {
                    throw new ArgumentException($"En passantsquare value of {FENparts[3]} is not valid", nameof(FEN));
                }
            }

            //Not all FENs include the last 2
            if (FENparts.Length < 5)
            {
                PositionHash.InitializeHash(this);
                return;
            }
            //50 move rule (in ply)
            if (int.TryParse(FENparts[4], out int plies))
            {
                PliesWithoutCapture = plies;
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
            PositionHash.InitializeHash(this);
            RepetitionTable.PushPosition(PositionHash.CurrentPositionHash);
        }
    }
}