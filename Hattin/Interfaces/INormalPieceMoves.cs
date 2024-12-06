using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface INormalPieceMoves
    {
        public List<BoardSquare> GeneratePawnMoves();
        public List<BoardSquare> GenerateKnightMoves();
        public List<BoardSquare> GenerateBishopMoves();
        public List<BoardSquare> GenerateRookMoves();
        public List<BoardSquare> GenerateQueenMoves();
        public List<BoardSquare> GenerateKingMoves();
    }
}