using HattinEngineLibrary.Types;

namespace HattinEngineLibrary.Utils
{
    public class MoveOrdering : Comparer<GeneratedMove>
    {
        public override int Compare(GeneratedMove? x, GeneratedMove? y)
        {
            //høffeligehter
            if (x is null && y is null) { return 0; }
            else if (x is null) { return -1; }
            else if (y is null) { return 1; }

            if (x.IsCheck == true && y.IsCheck == true) { return 0; }
            else if (x.IsCheck == true) { return 1; }
            else if (y.IsCheck == true) { return -1; }

            if (x.IsCapture == true && y.IsCapture == true) { return 0; }
            else if (x.IsCapture == true) { return 1; }
            else if (y.IsCapture == true) { return -1; }

            return 0;
        }
    }
}