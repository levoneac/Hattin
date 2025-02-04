using System.ComponentModel;
using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IEngine
    {
        public BoardState Board { get; init; }
        public IMoveGenerator MoveGenerator { get; init; }
        public MoveEvaluation GetNextMove();
        public List<GeneratedMove> GetPossibleMoves();
        public void AnalyzeCurrent(AnalyzedPosition analyzedPosition);
        public void AnalyzeCurrent(object? options);
        //public void InterruptSearch();
    }
}