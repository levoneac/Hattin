namespace Hattin.Utils
{
    //P is the parent class which contains the method F. P can be null if F is static.
    //F is a delegate, for example: Func<int, int>, which must match the function.
    //R is the returntype of the function (only keeps the last result)
    public class TimeFunction<P, F, R> where P : class where F : Delegate
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

        //Returns the time it took to run the test and the result of the last execution
        public (TimeSpan, R) RunTests()
        {
            var timeWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i-1 < TotalRuns; i++)
            {
                //Console.WriteLine(((List<GeneratedMove>)Function.Method.Invoke(ParentClass, Parameters))[0].Piece);
                Function.Method.Invoke(ParentClass, Parameters);
            }
            R res = (R)Function.Method.Invoke(ParentClass, Parameters);
            timeWatch.Stop();
            return (timeWatch.Elapsed, res);
        }
    }
}