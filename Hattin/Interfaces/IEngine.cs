using System.ComponentModel;
using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IEngine
    {
        public GeneratedMove GetNextMove();
        public void AnalyzeCurrent(int numberOfOptions, CancellationToken cancellationToken);
        public void AnalyzeCurrent(object? options);
        //public void InterruptSearch();
    }
}