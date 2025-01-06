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
            IsReady = false;
        }

        //Testing the cancellation token setup
        public string[] ExecuteCommand(string? input)
        {
            if (input is null || input == "")
            {
                return [""];
            }
            UCICommand GUICommands = UCIParser.GetUCICommand(input);

            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Uci)
            {
                UCIMode = true;
                string[] ret = ["id name Hattin", "id author Mathias G. Lien", "uciok"];
                return ret;
            }
            if (UCIMode == false)
            {
                return ["not in UCI mode"];
            }

            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Isready)
            {
                //Enginechecks
                IsReady = true;
                return [UCICommandFromEngine.Readyok.ToString().ToLower()];
            }
            if (IsReady == false)
            {
                return ["not ready"];
            }
            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Ucinewgame)
            {
                Engine.Board.ProcessFEN();
            }
            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Position)
            {
                //todo: handle castle and enpassant
                //Optimization: make incremental if next move is continuing from prev position
                Engine.Board.ProcessFEN();
                foreach (string moveString in GUICommands.Moves)
                {
                    //Needs to update pins and checks
                    Engine.Board.MovePiece(Move.GetMoveFromAlgebra(moveString, Engine.Board.PieceProperties));
                    //Engine.Board.PieceProperties.UpdateAllAttackSquares(Engine.MoveGenerator.GenerateAllAttackedSquares());
                    //Engine.Board.PrintAttackTotals(Engine.Board.SideToMove);
                    Engine.Board.PrintBoard(Engine.Board.SideToMove);
                }
            }
            if (GUICommands.CommandFromGUI == UCICommandFromGUI.Go)
            {
                if ((engineThread?.IsAlive ?? false) && (!cancellationToken?.IsCancellationRequested ?? false))
                {
                    return [""];
                }
                cancellationToken = new CancellationTokenSource();
                int numberOfOptions = 1;
                EngineIsActive = true;
                currentPosition = new AnalyzedPosition(numberOfOptions, Engine.Board.GetPositionHash(), cancellationToken.Token);

                engineThread = new Thread(Engine.AnalyzeCurrent);
                engineThread.Start(currentPosition);

                return [""];
            }
            else if (GUICommands.CommandFromGUI == UCICommandFromGUI.Stop && (engineThread?.IsAlive ?? false))
            {
                cancellationToken?.Cancel();
                return [""];
            }
            else if (GUICommands.CommandFromGUI == UCICommandFromGUI.Quit)
            {
                cancellationToken?.Cancel();
                ReadFromCLI = false;
            }

            return [""];
        }

        public void SendOutput(string[] output)
        {
            foreach (string item in output)
            {
                if (item != "")
                {
                    Console.WriteLine(item);
                }
            }
        }
        public void ReadLineThread(object? output)
        {//fix: stop thread when engine quits
            ((StringContainer)output).MyString = Console.ReadLine();
        }
        async public void StartListening()
        {
            StringContainer input = new StringContainer("");
            Thread? readThread = null;
            while (ReadFromCLI)
            {
                //Spawn another readline thread
                if (readThread is null || !readThread.IsAlive)
                {
                    //havent found another way to make this non blocking yet
                    readThread = new Thread(ReadLineThread);
                    readThread.Start(input);
                }

                if (input.MyString.Length > 0)
                {
                    SendOutput(ExecuteCommand(input.MyString));
                    input.MyString = "";
                }


                bool isEngineDone = (!engineThread?.IsAlive ?? false) || (currentPosition?.IsDone ?? false);
                if (EngineIsActive && isEngineDone)
                {
                    EngineIsActive = false;
                    Console.WriteLine($"bestmove {currentPosition.BestMove.Move.ToAlgebra()}");
                }
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