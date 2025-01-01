using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveConstraintBuilder
    {
        void Reset();
        void SetStopCheck();
        Func<List<GeneratedMove>, List<GeneratedMove>>? GetConstraintFunction();
    }
}