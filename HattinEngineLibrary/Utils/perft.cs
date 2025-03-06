using HattinEngineLibrary.Extensions.Move;
using HattinEngineLibrary.Interfaces;
using HattinEngineLibrary.Types;

namespace HattinEngineLibrary.Utils
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
                if (move.IsPromotion) { curResult.NumPromotions++; }

                Engine.Board.MovePiece(move);
                MoveGeneration(depth + 1);
                Engine.Board.UndoLastMove();
            }
        }

        public void PrintTotalMovesPerBranchTillDepth(int depth, string? FEN = null, bool onlyLastDepth = true)
        {
            MaxDepth = depth;

            if (FEN is not null)
            {
                Engine.Board.ProcessFEN(FEN);
            }
            long totalMoves = 0;
            long curBranchCount = 0;
            List<GeneratedMove> branches = Engine.GetPossibleMoves();
            for (int i = 0; i < branches.Count; i++)
            {
                GeneratedMove move = branches[i];
                //promote


                InitializeTotalPositoins(depth);
                Engine.Board.MovePiece(move);
                MoveGeneration(1);
                Engine.Board.UndoLastMove();

                curBranchCount = GetSumBranch(depth, onlyLastDepth);
                totalMoves += curBranchCount;
                Console.WriteLine($"Branch: {i + 1} - Move: {branches[i].ToAlgebra(true)} -> {curBranchCount}");

            }
            Console.WriteLine($"Total moves: {totalMoves}");
        }

        private long GetSumBranch(int depth, bool onlyLastDepth)
        {
            long curBranchCount = 0;
            if (onlyLastDepth)
            {
                curBranchCount = TotalCounts[depth - 1].NumMoves;
            }
            else
            {
                curBranchCount = TotalCounts.Sum(i => i.NumMoves);
            }
            return curBranchCount;
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

        //Depth: 1 -> Total: 20, Capt: 0, EP: 0, Cast: 0, Prom: 0, Check: 0
        //Depth: 2 -> Total: 400, Capt: 0, EP: 0, Cast: 0, Prom: 0, Check: 0
        //Depth: 3 -> Total: 8902, Capt: 34, EP: 0, Cast: 0, Prom: 0, Check: 12
        //Depth: 4 -> Total: 197281, Capt: 1576, EP: 0, Cast: 0, Prom: 0, Check: 469
        //Depth: 5 -> Total: 4865609, Capt: 82719, EP: 258, Cast: 0, Prom: 0, Check: 27345
        //Depth: 6 -> Total: 119060322, Capt: 2812006, EP: 5246, Cast: 0, Prom: 0, Check: 808770
        //Depth: 7 -> Total: -1099066995, Capt: 108328417, EP: 318113, Cast: 883453, Prom: 0, Check: 33085818
        //
        //real    166m22,124s
        //user    487m41,569s
        //sys     102m0,456s
        //

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
            public long Depth { get; set; } = 0;
            public long NumMoves { get; set; } = 0;
            public long NumEnPassant { get; set; } = 0;
            public long NumCaptures { get; set; } = 0;
            public long NumCasltes { get; set; } = 0;
            public long NumChecks { get; set; } = 0;
            public long NumPromotions { get; set; } = 0;
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