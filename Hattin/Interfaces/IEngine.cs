using System.ComponentModel;
using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IEngine
    {
        public BoardState Board { get; init; }
        public GeneratedMove GetNextMove();
        public void AnalyzeCurrent(AnalyzedPosition analyzedPosition);
        public void AnalyzeCurrent(object? options);
        //public void InterruptSearch();
    }
}