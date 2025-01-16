using Hattin.Extensions.Squares;

namespace Hattin.Types
{
    public class ZobristHash
    {
        public Dictionary<int, int> Hash { get; set; }
        public int CurrentPositionHash { get; private set; }
        private BoardSquare PreviouslySetEnPassantSquare { get; set; }
        private CastleRights PreviouslySetCastleRights { get; set; }
        public ZobristHash()
        {
            Hash = new Dictionary<int, int>();
            Random rnd = new Random();

            //Unique number for each piece and square combo
            foreach (NormalPiece piece in Enum.GetValues(typeof(NormalPiece)))
            {
                if (piece == NormalPiece.Empty) { continue; }
                foreach (BoardSquare square in Enum.GetValues(typeof(BoardSquare)))
                {
                    if (square == BoardSquare.NoSquare) { continue; }
                    Hash[GetKey(piece, square)] = rnd.Next();
                }
            }

            //Unique number for side == black
            Hash[GetKey(SideToMove.Black)] = rnd.Next();

            //Unique number for each castling combo 
            for (int i = 0; i < 16; i++)
            {
                Hash[i] = rnd.Next();
            }

            //Unique number for each enpassant file
            for (int i = 16; i < 24; i++)
            {
                Hash[i] = rnd.Next();
            }


        }

        //100 to 1398
        public static int GetKey(NormalPiece piece, BoardSquare square)
        {
            return ((int)piece * 1000) + (int)square; //fine as long as square is under 100 (i think)
        }

        //-1
        public static int GetKey(SideToMove sideToMove)
        {
            return (int)sideToMove;
        }

        //0 to 15
        public static int GetKey(CastleRights castleRights)
        {
            return (int)castleRights;
        }

        //16 to 23
        public static int GetKey(BoardSquare enPassantSquare)
        {
            return enPassantSquare.ToFileEnumValue();
        }

        public int InitializeHash(BoardState board)
        {
            PreviouslySetCastleRights = board.CastleRights;
            CurrentPositionHash = Hash[GetKey(PreviouslySetCastleRights)];

            PreviouslySetEnPassantSquare = board.EnPassantSquare;
            if (PreviouslySetEnPassantSquare != BoardSquare.NoSquare)
            {
                CurrentPositionHash ^= Hash[GetKey(PreviouslySetEnPassantSquare)];
            }

            if (board.SideToMove == SideToMove.Black) { CurrentPositionHash ^= Hash[GetKey(SideToMove.Black)]; }

            foreach (NormalPiece piece in Enum.GetValues(typeof(NormalPiece)))
            {
                if (piece == NormalPiece.Empty) { continue; }
                foreach (BoardSquare square in board.PieceProperties.PiecePositions[(int)piece])
                {
                    if (square == BoardSquare.NoSquare) { continue; }
                    CurrentPositionHash ^= Hash[GetKey(piece, square)];
                }
            }
            return CurrentPositionHash;
        }

        public int MovePiece(Move move, BoardState board)
        {
            //Remove piece
            CurrentPositionHash ^= Hash[GetKey(move.Piece, move.FromSquare)];

            //Remove captured piece
            NormalPiece pieceOnDestSquare = board.PieceProperties.GetPieceOnSquare(move.DestSquare);
            if (pieceOnDestSquare != NormalPiece.Empty)
            {
                CurrentPositionHash ^= Hash[GetKey(pieceOnDestSquare, move.DestSquare)];
            }
            //Enpassant capture
            else if (move.EnPassantCaptureSquare != BoardSquare.NoSquare)
            {
                NormalPiece pawn = board.PieceProperties.GetPieceOnSquare(move.EnPassantCaptureSquare);
                CurrentPositionHash ^= Hash[GetKey(pawn, move.EnPassantCaptureSquare)];
            }

            //Place piece 
            NormalPiece pieceAfterMove = move.PromoteTo == NormalPiece.Empty ? move.Piece : move.PromoteTo;
            CurrentPositionHash ^= Hash[GetKey(pieceAfterMove, move.DestSquare)];

            //Castle move
            if (move.RookCastleToSquare != BoardSquare.NoSquare && move.RookCastleFromSquare != BoardSquare.NoSquare)
            {
                NormalPiece rook = board.PieceProperties.GetPieceOnSquare(move.RookCastleFromSquare);
                //Remove rook
                CurrentPositionHash ^= Hash[GetKey(rook, move.RookCastleFromSquare)];

                //Place rook
                CurrentPositionHash ^= Hash[GetKey(rook, move.RookCastleToSquare)];
            }

            //Castle, side to move and ep
            UpdateState(board);

            return CurrentPositionHash;
        }

        public int UndoMove(PlayedMove move, BoardState board)
        {
            //Remove piece
            CurrentPositionHash ^= Hash[GetKey(move.PromotedToPiece, move.DestSquare)];

            //Place piece
            CurrentPositionHash ^= Hash[GetKey(move.PromotedFromPiece, move.FromSquare)];

            //Replace taken piece
            if (move.PieceOnDestSquare != NormalPiece.Empty)
            {
                CurrentPositionHash ^= Hash[GetKey(move.PieceOnDestSquare, move.DestSquare)];
            }
            if (move.EnPassantCaptureSquare != BoardSquare.NoSquare)
            {
                NormalPiece pawn = move.SideToMove == SideToMove.White ? NormalPiece.BlackPawn : NormalPiece.WhitePawn;
                CurrentPositionHash ^= Hash[GetKey(pawn, move.EnPassantCaptureSquare)];
            }
            //Castle move
            if (move.RookSourceSquare != BoardSquare.NoSquare && move.RookDestSquare != BoardSquare.NoSquare)
            {
                NormalPiece rook = board.PieceProperties.GetPieceOnSquare(move.RookDestSquare);
                CurrentPositionHash ^= Hash[GetKey(rook, move.RookDestSquare)];
                CurrentPositionHash ^= Hash[GetKey(rook, move.RookSourceSquare)];
            }

            //Castle, side to move and ep
            UpdateState(board);

            return CurrentPositionHash;

        }

        private void UpdateState(BoardState board)
        {
            //Castling rights
            if (board.CastleRights != PreviouslySetCastleRights)
            {
                //Remove old rights
                CurrentPositionHash ^= Hash[GetKey(PreviouslySetCastleRights)];

                //Add new rights
                PreviouslySetCastleRights = board.CastleRights;
                CurrentPositionHash ^= Hash[GetKey(PreviouslySetCastleRights)];
            }



            //Enpassant square
            if (board.EnPassantSquare != PreviouslySetEnPassantSquare)
            {
                //Remove old square
                if (PreviouslySetEnPassantSquare != BoardSquare.NoSquare)
                {
                    CurrentPositionHash ^= Hash[GetKey(PreviouslySetEnPassantSquare)];
                }

                PreviouslySetEnPassantSquare = board.EnPassantSquare;

                //Set new square
                if (PreviouslySetEnPassantSquare != BoardSquare.NoSquare)
                {
                    CurrentPositionHash ^= Hash[GetKey(PreviouslySetEnPassantSquare)];
                }

            }
            //Change sides
            CurrentPositionHash ^= Hash[GetKey(SideToMove.Black)];
        }
    }
}