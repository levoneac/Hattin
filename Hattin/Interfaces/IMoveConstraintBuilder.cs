using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IMoveConstraintBuilder
    {
        void Reset();
        void SetStopCheck();
        void SetPinRestriction();
        Func<List<GeneratedMove>, List<GeneratedMove>>? GetConstraintFunction();
    }
}