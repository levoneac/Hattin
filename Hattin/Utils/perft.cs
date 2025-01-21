using Hattin.Extensions.Move;
using Hattin.Extensions.NormalPiece;
using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Utils
{
    public class Perft
    {
        public IEngine Engine { get; set; }

        private int MaxDepth { get; set; }
        public List<PerftResult> TotalCounts { get; set; }
        public Perft(IEngine engine)
        {
            Engine = engine;
            TotalCounts = new List<PerftResult>();
        }
        private void MoveGeneration(int depth = 0)
        {
            PerftResult curResult;
            if (depth == MaxDepth)
            {
                return;
            }
            List<GeneratedMove> moves = Engine.GetPossibleMoves();
            curResult = TotalCounts[depth];
            foreach (GeneratedMove move in moves)
            {
                curResult.NumMoves++;
                if (move.IsCapture) { curResult.NumCaptures++; }
                if (move.IsCheck) { curResult.NumChecks++; }
                if (move.IsEnPassant) { curResult.NumEnPassant++; }
                if (move.RookCastleFromSquare != BoardSquare.NoSquare) { curResult.NumCasltes++; }
                if (move.IsPromotion)
                {
                    curResult.NumMoves += 3;
                    curResult.NumPromotions++;
                    foreach (NormalPiece promotion in NormalPieceClassifications.Promoteable)
                    {
                        if (promotion.ToColor() != Engine.Board.SideToMove) { continue; }
                        move.PromoteTo = promotion;
                        Engine.Board.MovePiece(move);
                        MoveGeneration(depth + 1);
                        Engine.Board.UndoLastMove();
                    }
                }
                Engine.Board.MovePiece(move);
                MoveGeneration(depth + 1);
                Engine.Board.UndoLastMove();
            }
        }

        public void PrintTotalMovesPerBranchTillDepth(int depth, string? FEN = null)
        {
            MaxDepth = depth;

            if (FEN is not null)
            {
                Engine.Board.ProcessFEN(FEN);
            }

            List<GeneratedMove> branches = Engine.GetPossibleMoves();
            for (int i = 0; i < branches.Count; i++)
            {
                InitializeTotalPositoins(depth);

                Engine.Board.MovePiece(branches[i]);
                MoveGeneration();
                Engine.Board.UndoLastMove();

                Console.WriteLine($"Move: {branches[i].ToAlgebra(true)} -> {TotalCounts.Sum(i => i.NumMoves)}");
            }
        }

        public void PrintTotalMovesTillDepth(int depth, string? FEN = null)
        {
            InitializeTotalPositoins(depth);
            MaxDepth = depth;

            if (FEN is not null)
            {
                Engine.Board.ProcessFEN(FEN);
            }
            else
            {
                Engine.Board.ProcessFEN();
            }

            MoveGeneration();

            for (int i = 0; i < TotalCounts.Count; i++)
            {
                PerftResult curResult = TotalCounts[i];
                Console.WriteLine($"Depth: {i + 1} -> Total: {curResult.NumMoves}, Capt: {curResult.NumCaptures}, EP: {curResult.NumEnPassant}, " +
                $"Cast: {curResult.NumCasltes}, Prom: {curResult.NumPromotions}, Check: {curResult.NumChecks}");
            }
        }
        //r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -
        //Results
        //Depth:0 -> 48
        //Depth:1 -> 2039
        //Depth:2 -> 97862
        //Depth:3 -> 4074278
        //Depth:4 -> 193439262

        //Expected
        //Depth:0 -> 48
        //Depth:1 -> 2039
        //Depth:2 -> 97862
        //Depth:3 -> 4085603
        //Depth:4 -> 193690690

        //Issues seem to be castles and maybe promotions and checkmates

        //rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8  


        private void InitializeTotalPositoins(int depth)
        {
            TotalCounts.Clear();
            for (int i = 0; i < depth; i++)
            {
                TotalCounts.Add(new PerftResult(i));
            }
        }

        public class PerftResult
        {
            public int Depth { get; set; } = 0;
            public int NumMoves { get; set; } = 0;
            public int NumEnPassant { get; set; } = 0;
            public int NumCaptures { get; set; } = 0;
            public int NumCasltes { get; set; } = 0;
            public int NumChecks { get; set; } = 0;
            public int NumPromotions { get; set; } = 0;
            public PerftResult(int depth)
            {
                Depth = depth;
            }
        }

        //private void MoveGeneration(int depth = 0)
        //{
        //    if (depth == MaxDepth - 1)
        //    {
        //        TotalPositions[depth] += Engine.GetPossibleMoves().Count;
        //        return;
        //    }
        //    List<GeneratedMove> moves = Engine.GetPossibleMoves();
        //    TotalPositions[depth] += moves.Count;
        //    foreach (GeneratedMove move in moves)
        //    {
        //        if (move.IsPromotion)
        //        {
        //            TotalPositions[depth] += 3;
        //            foreach (NormalPiece promotion in NormalPieceClassifications.Promoteable)
        //            {
        //                if (promotion.ToColor() != Engine.Board.SideToMove) { continue; }
        //                move.PromoteTo = promotion;
        //                Engine.Board.MovePiece(move);
        //                MoveGeneration(depth + 1);
        //                Engine.Board.UndoLastMove();
        //            }
        //        }
        //        Engine.Board.MovePiece(move);
        //        MoveGeneration(depth + 1);
        //        Engine.Board.UndoLastMove();
        //    }
        //}
    }
}