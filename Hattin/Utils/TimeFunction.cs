using Hattin.Types;

namespace Hattin.Utils
{
    public class TimeFunction<P, F> where P : class where F : Delegate
    {
        public int TotalRuns { get; set; }
        public P ParentClass { get; set; }
        public F Function { get; set; }
        public object[] Parameters { get; set; }

        public TimeFunction(P parentClass, F function, int totalRuns = 10, params object[] parameters)
        {
            ParentClass = parentClass;
            Function = function;
            TotalRuns = totalRuns;
            Parameters = parameters;
        }

        public TimeSpan RunTests()
        {
            var timeWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < TotalRuns; i++)
            {
                //Console.WriteLine(((List<GeneratedMove>)Function.Method.Invoke(ParentClass, Parameters))[0].Piece);
                Function.Method.Invoke(ParentClass, Parameters);
            }
            timeWatch.Stop();
            return timeWatch.Elapsed;
        }
    }
}