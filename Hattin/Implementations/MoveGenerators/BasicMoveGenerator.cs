using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.MoveGenerators
{
    public class BasicMoveGenerator : IMoveGenerator
    {

        private static List<BoardSquare> GenerateSlidingMoves(BoardState currentBoard, NormalPiece piece, SideToMove opponentColor)
        {
            List<BoardSquare> possibleMoves = [];
            BoardSquare positionAfterOffset;

            foreach (int bishopPosition in currentBoard.PieceProperties.PiecePositions[(int)piece].Select(i => (int)i))
            {
                foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
                {
                    positionAfterOffset = (BoardSquare)(bishopPosition + offset);

                    while ((BoardSquare)Conversions.SquareConversions.Convert120To64((int)positionAfterOffset) != BoardSquare.NoSquare)
                    {
                        SideToMove colorOfPieceOnSquare = currentBoard.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
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

        private static List<BoardSquare> GenerateJumpingMoves(BoardState currentBoard, NormalPiece piece, SideToMove opponentColor)
        {
            List<BoardSquare> possibleMoves = [];
            BoardSquare positionAfterOffset;
            BoardSquare checkIfNoSquare;

            foreach (int piecePosition in currentBoard.PieceProperties.PiecePositions[(int)piece].Select(i => (int)i))
            {
                foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
                {
                    positionAfterOffset = (BoardSquare)(piecePosition + offset);

                    checkIfNoSquare = (BoardSquare)Conversions.SquareConversions.Convert120To64((int)positionAfterOffset);
                    if (checkIfNoSquare != BoardSquare.NoSquare)
                    {
                        SideToMove colorOfPieceOnSquare = currentBoard.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
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
        public List<BoardSquare> GenerateKnightMoves(BoardState currentBoard)
        {
            NormalPiece pieceColor = currentBoard.SideToMove == SideToMove.White ? NormalPiece.WhiteKnight : NormalPiece.BlackKnight;
            SideToMove opponentColor = currentBoard.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateJumpingMoves(currentBoard, pieceColor, opponentColor);
        }

        public List<BoardSquare> GeneratePawnMoves(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public List<BoardSquare> GenerateBishopMoves(BoardState currentBoard)
        {
            NormalPiece pieceColor = currentBoard.SideToMove == SideToMove.White ? NormalPiece.WhiteBishop : NormalPiece.BlackBishop;
            SideToMove opponentColor = currentBoard.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateSlidingMoves(currentBoard, pieceColor, opponentColor);

        }

        public List<BoardSquare> GenerateRookMoves(BoardState currentBoard)
        {
            NormalPiece pieceColor = currentBoard.SideToMove == SideToMove.White ? NormalPiece.WhiteRook : NormalPiece.BlackRook;
            SideToMove opponentColor = currentBoard.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateSlidingMoves(currentBoard, pieceColor, opponentColor);
        }

        public List<BoardSquare> GenerateQueenMoves(BoardState currentBoard)
        {
            NormalPiece pieceColor = currentBoard.SideToMove == SideToMove.White ? NormalPiece.WhiteQueen : NormalPiece.BlackQueen;
            SideToMove opponentColor = currentBoard.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateSlidingMoves(currentBoard, pieceColor, opponentColor);
        }

        public List<BoardSquare> GenerateKingMoves(BoardState currentBoard)
        {
            NormalPiece pieceColor = currentBoard.SideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            SideToMove opponentColor = currentBoard.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            return GenerateJumpingMoves(currentBoard, pieceColor, opponentColor);
        }

        public Move GenerateNextValidMove(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }
        public Move GenerateNextCapture(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public Move GenerateNextCheck(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }
    }

}