using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.MoveGenerators
{
    public class BasicMoveGenerator : IMoveGenerator
    {
        public BoardState Board { get; private set; } //make into interface later
        public BasicMoveGenerator(BoardState board)
        {
            Board = board;
        }
        private List<BoardSquare> GenerateSlidingMoves(NormalPiece piece, SideToMove opponentColor)
        {
            List<BoardSquare> possibleMoves = [];
            BoardSquare positionAfterOffset;

            //foreach square where a given piece is placed
            foreach (BoardSquare piecePosition in Board.PieceProperties.PiecePositions[(int)piece])
            {
                //foreach offset of the given piece
                foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
                {
                    positionAfterOffset = piecePosition + offset;

                    //while the square is on the board, check if the piece collides with something
                    while ((BoardSquare)Conversions.SquareConversions.Convert120To64((int)positionAfterOffset) != BoardSquare.NoSquare)
                    {
                        SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                        if (colorOfPieceOnSquare == opponentColor)
                        {
                            //set capture flag
                            possibleMoves.Add(positionAfterOffset);
                            break;
                        }
                        else if (colorOfPieceOnSquare == SideToMove.None)
                        {
                            possibleMoves.Add(positionAfterOffset);
                            positionAfterOffset += offset;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return possibleMoves;
        }

        private List<BoardSquare> GenerateJumpingMoves(NormalPiece piece, SideToMove opponentColor)
        {
            List<BoardSquare> possibleMoves = [];
            BoardSquare positionAfterOffset;
            BoardSquare checkIfNoSquare;

            foreach (BoardSquare piecePosition in Board.PieceProperties.PiecePositions[(int)piece])
            {
                foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
                {
                    positionAfterOffset = piecePosition + offset;

                    checkIfNoSquare = (BoardSquare)Conversions.SquareConversions.Convert120To64((int)positionAfterOffset);
                    if (checkIfNoSquare != BoardSquare.NoSquare)
                    {
                        SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                        if (colorOfPieceOnSquare == opponentColor)
                        {
                            //set capture flag
                            possibleMoves.Add(positionAfterOffset);
                        }
                        else if (colorOfPieceOnSquare == SideToMove.None)
                        {
                            possibleMoves.Add(positionAfterOffset);
                        }
                    }
                }
            }
            return possibleMoves;
        }
        public List<BoardSquare> GenerateKnightMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKnight : NormalPiece.BlackKnight;
            SideToMove opponentColor = Board.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateJumpingMoves(pieceColor, opponentColor);
        }

        //nearing spaghetti
        public List<BoardSquare> GeneratePawnMoves()
        {
            List<BoardSquare> possibleMoves = [];
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhitePawn : NormalPiece.BlackPawn;
            SideToMove opponentColor = Board.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            BoardSquare positionAfterOffset;
            BoardSquare checkIfNoSquare;
            BoardSquare[] startingSquares = NormalPieceStartingSquares.GetStartingSquareFromNormalPiece(pieceColor);
            BoardSquare[] promotionSquares = NormalPiecePromotionSquares.GetPromotionSquareFromNormalPiece(pieceColor);

            foreach (BoardSquare pawnPosition in Board.PieceProperties.PiecePositions[(int)pieceColor])
            {
                bool prevIsBlocked = false;
                int offsetIndex = 1; //if pawn is not on starting square, skip the first index of offsets. Maybe find a clearer way to represent this
                if (startingSquares.Contains(pawnPosition))
                {
                    offsetIndex = 0;
                }
                //get correct color offsets for pawn 
                int[] offsets = NormalPieceOffsets.GetOffsetFromNormalPiece(pieceColor);

                //loop over each of the offsets for each pawn
                for (; offsetIndex < offsets.Length; offsetIndex++)
                {
                    positionAfterOffset = pawnPosition + offsets[offsetIndex];

                    //if the move doesnt put the pawn off board
                    checkIfNoSquare = (BoardSquare)Conversions.SquareConversions.Convert120To64((int)positionAfterOffset);
                    if (checkIfNoSquare != BoardSquare.NoSquare)
                    {
                        //if its an attacking move and (square is occupied by opponent or it is an enpassant square)
                        SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                        if (offsetIndex > 1 && (colorOfPieceOnSquare == opponentColor || positionAfterOffset == Board.EnPassantSquare))
                        {
                            //set capture flag
                            possibleMoves.Add(positionAfterOffset);
                        }
                        //else if the square is not occupied and its a normal forward move
                        else if (colorOfPieceOnSquare == SideToMove.None && offsetIndex < 2)
                        {
                            //if its a double move, check if the pawn moved into another piece
                            if (offsetIndex == 0)
                            {
                                SideToMove pieceBehind = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset - offsets[1]);
                                if (pieceBehind != SideToMove.None)
                                {
                                    prevIsBlocked = true;
                                }

                                //set enpassant square as -10 or +10 from this square
                            }

                            //check if destination is a promotion square and set promotion flag
                            if (offsetIndex == 1 && promotionSquares.Contains(positionAfterOffset))
                            {
                                //promotion flag
                            }

                            if (!prevIsBlocked)
                            {
                                possibleMoves.Add(positionAfterOffset);
                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }

        public List<BoardSquare> GenerateBishopMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteBishop : NormalPiece.BlackBishop;
            SideToMove opponentColor = Board.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateSlidingMoves(pieceColor, opponentColor);

        }

        public List<BoardSquare> GenerateRookMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteRook : NormalPiece.BlackRook;
            SideToMove opponentColor = Board.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateSlidingMoves(pieceColor, opponentColor);
        }

        public List<BoardSquare> GenerateQueenMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteQueen : NormalPiece.BlackQueen;
            SideToMove opponentColor = Board.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateSlidingMoves(pieceColor, opponentColor);
        }

        public List<BoardSquare> GenerateKingMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            SideToMove opponentColor = Board.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateJumpingMoves(pieceColor, opponentColor);
        }

        public Move GenerateNextValidMove()
        {
            throw new NotImplementedException();
        }
        public Move GenerateNextCapture()
        {
            throw new NotImplementedException();
        }

        public Move GenerateNextCheck()
        {
            throw new NotImplementedException();
        }
    }

}