using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface INormalPieceMoves
    {
        public List<GeneratedMove> GeneratePawnMoves();
        public List<GeneratedMove> GenerateKnightMoves();
        public List<GeneratedMove> GenerateBishopMoves();
        public List<GeneratedMove> GenerateRookMoves();
        public List<GeneratedMove> GenerateQueenMoves();
        public List<GeneratedMove> GenerateKingMoves();
    }
}