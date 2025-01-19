using Hattin.Extensions.NormalPiece;
using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Utils
{
    public class Perft
    {
        public IEngine Engine { get; set; }
        public List<long> TotalPositions { get; set; }
        private int MaxDepth { get; set; }
        public Perft(IEngine engine)
        {
            Engine = engine;
            TotalPositions = new List<long>();
        }
        private void MoveGeneration(int depth = 0)
        {
            if (depth == MaxDepth - 1)
            {
                TotalPositions[depth] += Engine.GetPossibleMoves().Count;
                return;
            }
            List<GeneratedMove> moves = Engine.GetPossibleMoves();
            TotalPositions[depth] += moves.Count;
            foreach (GeneratedMove move in moves)
            {
                if (move.IsPromotion)
                {
                    TotalPositions[depth] += 3;
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

        public void PrintTotalMovesTillDepth(int depth, string? FEN = null)
        {
            TotalPositions.Clear();
            for (int i = 0; i < depth; i++)
            {
                TotalPositions.Add(0);
            }
            MaxDepth = depth;

            if (FEN is not null)
            {
                Engine.Board.ProcessFEN(FEN);
            }
            MoveGeneration();
            for (int i = 0; i < TotalPositions.Count; i++)
            {
                Console.WriteLine($"Depth:{i} -> {TotalPositions[i]}");
            }
        }
        //r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq 
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

    }
}