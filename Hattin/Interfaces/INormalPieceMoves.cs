using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface INormalPieceMoves
    {
        public List<BoardSquare> GeneratePawnMoves(BoardState currentBoard);
        public List<BoardSquare> GenerateKnightMoves(BoardState currentBoard);
        public List<BoardSquare> GenerateBishopMoves(BoardState currentBoard);
        public List<BoardSquare> GenerateRookMoves(BoardState currentBoard);
        public List<BoardSquare> GenerateQueenMoves(BoardState currentBoard);
        public List<BoardSquare> GenerateKingMoves(BoardState currentBoard);
    }
}