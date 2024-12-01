using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.MoveGenerators
{
    public class BasicMoveGenerator : IMoveGenerator
    {


        public List<BoardSquare> GenerateKnightMoves(BoardState currentBoard)
        {
            List<BoardSquare> possibleMoves = [];
            NormalPiece knightColor = currentBoard.SideToMove == SideToMove.White ? NormalPiece.WhiteKnight : NormalPiece.BlackKnight;
            SideToMove opponentColor = currentBoard.SideToMove == SideToMove.White ? SideToMove.Black : SideToMove.White;
            BoardSquare positionAfterOffset;
            BoardSquare checkIfNoSquare;

            foreach (int knightPosition in currentBoard.PieceProperties.PiecePositions[(int)knightColor].Select(i => (int)i))
            {
                foreach (int offset in NormalPieceOffsets.Knight)
                {
                    positionAfterOffset = (BoardSquare)(knightPosition + offset);

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

        public List<BoardSquare> GeneratePawnMoves(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public List<BoardSquare> GenerateBishopMoves(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public List<BoardSquare> GenerateRookMoves(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public List<BoardSquare> GenerateQueenMoves(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }

        public List<BoardSquare> GenerateKingMoves(BoardState currentBoard)
        {
            throw new NotImplementedException();
        }
    }
}