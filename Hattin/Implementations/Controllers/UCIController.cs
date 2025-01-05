using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Hattin.Extensions;
using Hattin.Implementations.Parsers;
using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.Controllers
{
    public class UCIController : IUCIController
    {
        //public IUCIParser Parser;
        public IEngine Engine;
        private Thread? engineThread;
        private AnalyzedPosition? currentPosition;
        private CancellationTokenSource? cancellationToken;
        private bool ReadFromCLI;
        private bool UCIMode;
        private bool IsReady;
        private bool EngineIsActive;
        public UCIController(IEngine engine)
        {
            //Parser = parser;
            Engine = engine;
            ReadFromCLI = true;
            UCIMode = false;
            EngineIsActive = false;
        }

        //Testing the cancellation token setup
        public string ExecuteCommand(string? input)
        {
            if (input is null || input == "")
            {
                return "";
            }
            UCICommand GUICommands = UCIParser.GetUCICommand(input);

            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Uci)
            {
                UCIMode = true;
                return @"id name Hattin
                id author Mathias G. Lien
                uciok";
            }
            if (UCIMode == false)
            {
                return "Not in UCI mode";
            }
            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Isready)
            {
                //Enginechecks
                return UCICommandFromEngine.Readyok.ToString().ToLower();
            }

            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Go)
            {
                if ((engineThread?.IsAlive ?? false) && (!cancellationToken?.IsCancellationRequested ?? false))
                {
                    return "Already running";
                }
                cancellationToken = new CancellationTokenSource();
                int numberOfOptions = 1;
                EngineIsActive = true;
                currentPosition = new AnalyzedPosition(numberOfOptions, Engine.Board.GetPositionHash(), cancellationToken.Token);

                engineThread = new Thread(Engine.AnalyzeCurrent);
                engineThread.Start(currentPosition);

                return "STARTED";
            }
            else if (GUICommands.CommandFromGUI == UCICommandFromGUI.Stop && (engineThread?.IsAlive ?? false))
            {
                cancellationToken?.Cancel();
                return "REQUESTED STOP";
            }
            else if (GUICommands.CommandFromGUI == UCICommandFromGUI.Quit)
            {
                cancellationToken?.Cancel();
                ReadFromCLI = false;
            }

            return $"";
        }

        public void SendOutput(string output)
        {
            throw new NotImplementedException();
        }
        public void ReadLineThread(object? output)
        {
            ((StringContainer)output).MyString = Console.ReadLine();
        }
        async public void StartListening()
        {
            StringContainer input = new StringContainer("");
            Thread? readThread = null;
            while (ReadFromCLI)
            {
                if (readThread is null || !readThread.IsAlive)
                {
                    //havent found another way to make this non blocking yet
                    readThread = new Thread(ReadLineThread);
                    readThread.Start(input);
                }
                if (input.MyString.Length > 0)
                {
                    Console.WriteLine(ExecuteCommand(input.MyString));
                    input.MyString = "";
                }

                bool isEngineDone = (!engineThread?.IsAlive ?? false) || (currentPosition?.IsDone ?? false);
                if (EngineIsActive && isEngineDone)
                {
                    EngineIsActive = false;
                    Console.WriteLine($"bestmove {currentPosition.BestMove.Move.ToAlgebra()}");
                }
                Thread.Sleep(500);
            }
        }
        //Still blocking :(
        //byte[] buffer = new byte[1024];
        //Stream inputStream = Console.OpenStandardInput();
        //int numberOfCharsRead = 0;
        //string input;
        //numberOfCharsRead = inputStream.Read(buffer, 0, 1024);
        //if (numberOfCharsRead == 0)
        //{
        //    continue;
        //}
        //input = string.Concat(Encoding.UTF8.GetChars(buffer, 0, numberOfCharsRead));
        //input = input.Remove(input.IndexOf(Environment.NewLine.ToString()));
    }
}