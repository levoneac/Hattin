using System.Collections.Concurrent;
using Hattin.Extensions.Move;
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

        public string[] ExecuteCommand(string? input)
        {
            if (input is null || input == "")
            {
                return [""];
            }

            //debug
            //if (input == "undo")
            //{
            //    Engine.Board.UndoLastMove();
            //    Engine.Board.PrintBoard(Engine.Board.SideToMove);
            //}

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
                //Optimization: make incremental if next move is continuing from prev position
                if (GUICommands.FEN is not null)
                {
                    Engine.Board.ProcessFEN(GUICommands.FEN);
                }
                else
                {
                    Engine.Board.ProcessFEN(); //resets the board to startpos
                }

                foreach (string moveString in GUICommands.Moves)
                {
                    Engine.Board.MovePiece(Move.GetMoveFromAlgebra(moveString, Engine.Board));
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
                currentPosition = new AnalyzedPosition(numberOfOptions, Engine.Board.PositionHash.CurrentPositionHash, cancellationToken.Token);
                Engine.Board.PieceProperties.UpdateAllAttackSquares(Engine.MoveGenerator.GenerateAllAttackedSquares());
                Engine.Board.PrintAttackTotals(Engine.Board.SideToMove);
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
        {
            ((ConcurrentQueue<string>)output).Enqueue(Console.ReadLine());
        }

        public void StartListening()
        {
            ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

            Thread? readThread = null;
            while (ReadFromCLI)
            {
                //Spawn another readline thread
                if (readThread is null || !readThread.IsAlive)
                {
                    //havent found another way to make this non blocking yet
                    readThread = new Thread(ReadLineThread);
                    readThread.Start(messageQueue);
                }

                if (messageQueue.TryDequeue(out string? command))
                {
                    SendOutput(ExecuteCommand(command));
                }


                bool isEngineDone = (!engineThread?.IsAlive ?? false) || (currentPosition?.IsDone ?? false);
                if (EngineIsActive && isEngineDone)
                {
                    EngineIsActive = false;
                    Console.WriteLine($"bestmove {currentPosition.BestMove.Move.ToAlgebra()}");
                    Engine.Board.PrintBoard(Engine.Board.SideToMove);
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