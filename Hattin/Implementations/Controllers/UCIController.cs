using System.ComponentModel;
using Hattin.Interfaces;

namespace Hattin.Implementations.Controllers
{
    public class UCIController : BackgroundWorker, IUCIController
    {
        public IEngine Engine;
        private Thread? engineThread;
        private CancellationTokenSource? cancellationToken;
        public UCIController(IEngine engine)
        {
            Engine = engine;
        }

        //Testing the cancellation token setup
        public string? ParseInput(string? input)
        {
            if (input is null)
            {
                return "NO DATA";
            }
            if (input.Contains("RUN"))
            {
                if ((engineThread?.IsAlive ?? false) && (!cancellationToken?.IsCancellationRequested ?? false))
                {
                    return "Already running";
                }

                cancellationToken = new CancellationTokenSource();
                int numberOfOptions = 1;
                engineThread = new Thread(Engine.AnalyzeCurrent);
                engineThread.Start((numberOfOptions, cancellationToken.Token));
                return "STARTED";
            }
            else if (input.Contains("STOP") && (engineThread?.IsAlive ?? false))
            {
                cancellationToken?.Cancel();
                return "REQUESTED STOP";
            }
            return $"{input} is an unknown command";
        }

        public void SendOutput(string output)
        {
            throw new NotImplementedException();
        }

        public void StartListening()
        {
            while (true)
            {
                string? input = Console.ReadLine();
                Console.WriteLine(ParseInput(input));
            }
        }
    }
}