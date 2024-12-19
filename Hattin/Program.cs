using Hattin.Engine;
using Hattin.Extensions.SideToMove;
using Hattin.Extensions.SquareInteraction;
using Hattin.Extensions.Squares;
using Hattin.Implementations.MoveGenerators;
using Hattin.Implementations.PositionEvaluators;
using Hattin.Interfaces;
using Hattin.Types;
using Hattin.Utils;

namespace Hattin
{

    class Program
    {
        public static void Main(string[] args)
        {
            /*
                //Lists dont copy the item, its still a reference
                //Simple types (primitives?) like ints do get copied and cant be changed from outside
                List<int[]> a = new();
                int[] b = new int[1];
                int x = 21;
                b[0] = x;
                a.Add(b);
                Console.WriteLine(a[0][0]); //21
                x = 21344;
                b[0] = 4124;
                Console.WriteLine(a[0][0]); //4124
            */

            BitBoard bb = new();
            // /bb.Board = 256 + 512 + 1024 + 2048 + 4096 + 8192 + 16384 + 32768;
            // /bb.PrintBitBoard();
            // bb.Board |= 1UL << Conversions.SquareConversions.Array120To64[(int)BoardSquares.H8];
            // /bb.PrintBitBoard();
            // /bb.SetBitPieceBase_120(23);
            // /bb.PrintBitBoard();
            // /Console.WriteLine(bb.CountSetBits());
            // /BitBoard.PrintBitBoard(bb.GetLastSetBit());
            // /Console.WriteLine(bb.LastSetBitIndex());
            // /bb.PopLastSetBit();
            // /bb.PrintBitBoard();
            // /bb.PopLastSetBit();
            // /bb.PrintBitBoard();
            // /bb.PopLastSetBit();
            // /bb.PrintBitBoard();
            // /bb.PopLastSetBit();
            // /bb.PrintBitBoard();

            ulong d = (ulong)new Random().NextInt64();
            BoardState board = new BoardState();
            IMoveGenerator generator = new BasicMoveGenerator(board);
            IPositionEvaluator evaluator = new BasicPositionEvaluator();
            HattinEngine0_1 engine0_1 = new HattinEngine0_1(board, generator, evaluator);
            board.ProcessFEN("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2 ");
            board.PrintBoard(SideToMove.White, false);
            board.PrintBoard(SideToMove.Black, false);
            Console.WriteLine(board.GetPositionHash());
            board.GetMoveHistory();
            board.MovePiece(NormalPiece.BlackKnight, BoardSquare.G8, BoardSquare.D3);
            board.MovePiece(NormalPiece.WhitePawn, BoardSquare.D2, BoardSquare.D5);

            //board.PieceProperties.RemovePiece(NormalPiece.BlackPawn, BoardSquare.H7);
            for (int i = 0; i < board.PieceProperties.PiecePositions.Count; i++)
            {
                Console.Write($"{(NormalPiece)i}: ");
                for (int j = 0; j < board.PieceProperties.PiecePositions[i].Count; j++)
                {
                    Console.Write($"{board.PieceProperties.PiecePositions[i][j]}, ");
                }
                Console.WriteLine();
            }
            PieceTotals total = board.PieceProperties.CalculatePieceTotals();
            Console.WriteLine("white: {0}, black: {1}", total.white, total.black);


            board.MovePiece(NormalPiece.BlackPawn, BoardSquare.E7, BoardSquare.E2);
            board.MovePiece(NormalPiece.WhiteKnight, BoardSquare.F3, BoardSquare.H4);
            board.MovePiece(NormalPiece.BlackKing, BoardSquare.E8, BoardSquare.E7);
            //board.EnPassantSquare = BoardSquare.E6;
            //List<GeneratedMove> kMoves = engine0_1.MoveGenerator.GeneratAllLegalMoves();
            //Console.WriteLine("___Moves___: ");
            //foreach (GeneratedMove move in kMoves)
            //{
            //    Console.Write($"{move.FromSquare}-{move.DestSquare}, EP-square: {move.EnPassantSquare}, promotion?: {move.IsPromotion}, check?: {move.IsCheck}, capture?: {move.IsCapture}, attacked squares: ");
            //    move.AttackedSquares.ForEach(i => Console.Write($"({i.AsPiece}->{i.Square}:{i.PieceOnSquare}-{i.Interaction.ToShortString()}{(i.IsPromotion ? "++" : "")}) "));
            //    Console.WriteLine();
            //}
            //Console.WriteLine();


            //var timer = new TimeFunction<IMoveGenerator, Func<List<GeneratedMove>>, List<GeneratedMove>>(engine0_1.MoveGenerator, engine0_1.MoveGenerator.GeneratAllLegalMoves, 1000);
            //var (timerResult, functionResult) = timer.RunTests();
            //Console.WriteLine(timerResult);

            IMoveGenerator threadedGenerator = new BasicMoveGeneratorThreaded(board);
            HattinEngine0_1 engineThreaded = new HattinEngine0_1(board, threadedGenerator, evaluator);

            var timer2 = new TimeFunction<IMoveGenerator, Func<List<GeneratedMove>>, List<GeneratedMove>>(engineThreaded.MoveGenerator, engineThreaded.MoveGenerator.GenerateAllLegalMoves, 1);
            var (timerResult2, functionResult2) = timer2.RunTests();
            Console.WriteLine(timerResult2);


            var timer3 = new TimeFunction<IMoveGenerator, Func<List<AttackProjection>>, List<AttackProjection>>(engineThreaded.MoveGenerator, engineThreaded.MoveGenerator.GenerateAllAttackedSquares, 1);
            var (timerResult3, functionResult3) = timer3.RunTests();
            Console.WriteLine(timerResult3);

            List<AttackProjection> attacks = engineThreaded.MoveGenerator.GenerateAllAttackedSquares();
            board.PieceProperties.UpdateAllAttackSquares(attacks);
            board.PrintBoard(SideToMove.Black);
            board.PrintAttackTotals(SideToMove.Black);

            //SquareRange.GetSquaresBetween(BoardSquare.A8, BoardSquare.A1, AbsoluteDirectionalOffsets.Row, false).ForEach(s => Console.Write($"{s}, "));
            //Console.WriteLine();
            //SquareRange.GetSquaresBetween(BoardSquare.A1, BoardSquare.A8, AbsoluteDirectionalOffsets.Row, true).ForEach(s => Console.Write($"{s}, "));
            //
            //SquareRange.GetSquaresBetween(BoardSquare.A8, BoardSquare.H8, AbsoluteDirectionalOffsets.Column, true).ForEach(s => Console.Write($"{s}, "));
            //Console.WriteLine();
            //SquareRange.GetSquaresBetween(BoardSquare.E3, BoardSquare.A3, AbsoluteDirectionalOffsets.Column, false).ForEach(s => Console.Write($"{s}, "));


            //foreach (var move in functionResult2)
            //{
            //    Console.WriteLine($"{move.Piece}:{move.FromSquare}-{move.DestSquare}");
            //}

            //Console.WriteLine(functionResult.Count == functionResult2.Count);

            /*
                |R||N||B||K||Q||B||N||R|
                |P||P||P||P||P||P||P||P|
                |0||0||0||0||0||0||0||0|
                |0||0||0||0||0||0||0||0|
                |0||0||0||0||0||0||0||0|
                |0||0||0||0||0||0||0||0|
                |p||p||p||p||p||p||p||p|
                |r||n||b||k||q||b||n||r|
            */


            /*
            Console.WriteLine();
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write("|" + Conversions.SquareConversions.Array64To120[i] + "|");
            }

            Console.WriteLine();
            for (int i = 21; i < 99; i++)
            {
                int conv = Conversions.SquareConversions.Array120To64[i];
                if (conv % 8 == 0 && conv != 112)
                {
                    Console.WriteLine();
                }
                Console.Write("|" + conv + "|");
            }
            */

            //TimeFunction usage:
            //int Test(int x, int y, int z)
            //{
            //    return x + y + z;
            //}
            //var timer = new TimeFunction<object, Func<int, int, int, int>>(null, Test, 1, 1, 5, 3);
            //null -> the parent class of the function
        }
    }
}
//Console.WriteLine(-27.ConvertBoardIndexing(SquareIndexType.Base_120)); //the minus is considered after the conversion happens