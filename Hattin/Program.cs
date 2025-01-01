using Hattin.Engine;
using Hattin.Extensions.SideToMove;
using Hattin.Extensions.SquareInteraction;
using Hattin.Extensions.Squares;
using Hattin.Implementations.MoveConstraintBuilders;
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
            BoardState board = new BoardState();
            IPositionEvaluator evaluator = new BasicPositionEvaluator();
            IMoveConstraintBuilder constraintBuilder = new BasicMoveConstraintBuilder(board);
            board.ProcessFEN("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2 ");
            board.PrintBoard(SideToMove.White, false);
            board.PrintBoard(SideToMove.Black, false);
            Console.WriteLine(board.GetPositionHash());
            board.GetMoveHistory();
            //board.MovePiece(NormalPiece.BlackKnight, BoardSquare.G8, BoardSquare.D3);
            //board.MovePiece(NormalPiece.WhitePawn, BoardSquare.D2, BoardSquare.D5);

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


            board.MovePiece(new Move(NormalPiece.BlackPawn, BoardSquare.E7, BoardSquare.E5));
            board.MovePiece(new Move(NormalPiece.WhiteBishop, BoardSquare.F1, BoardSquare.B5));
            board.MovePiece(new Move(NormalPiece.BlackPawn, BoardSquare.H7, BoardSquare.H5));
            board.MovePiece(new Move(NormalPiece.WhiteBishop, BoardSquare.B5, BoardSquare.D7));
            //board.MovePiece(NormalPiece.BlackPawn, BoardSquare.H7, BoardSquare.H5);
            //board.MovePiece(NormalPiece.WhiteBishop, BoardSquare.C1, BoardSquare.G5);
            //board.MovePiece(NormalPiece.BlackKing, BoardSquare.E7, BoardSquare.E8);
            //board.EnPassantSquare = BoardSquare.E6;

            IMoveGenerator threadedGenerator = new BasicMoveGeneratorThreaded(board);
            HattinEngine0_1 engineThreaded = new HattinEngine0_1(board, threadedGenerator, constraintBuilder, evaluator);


            //
            //
            //List<GeneratedMove> kMoves = engineThreaded.MoveGenerator.GenerateKingMoves();
            //Console.WriteLine("___Moves___: ");


            //var timer3 = new TimeFunction<IMoveGenerator, Func<List<AttackProjection>>, List<AttackProjection>>(engineThreaded.MoveGenerator, engineThreaded.MoveGenerator.GenerateAllAttackedSquares, 1000);
            //var (timerResult3, functionResult3) = timer3.RunTests();
            //Console.WriteLine(timerResult3);


            //engineThreaded.PlayUntillPly(10);

            //List<List<AttackProjection>> attacks = engineThreaded.MoveGenerator.GenerateAllAttackedSquares();
            //board.PieceProperties.UpdateAllAttackSquares(attacks);
            board.PrintBoard(SideToMove.White);


            board.PieceProperties.UpdateAllAttackSquares(engineThreaded.MoveGenerator.GenerateAllAttackedSquares());
            board.PrintAttackTotals(SideToMove.White);


            //SAVE FOR LOGGING LATER
            foreach (var pin in board.PieceProperties.GetPinnedPieces([NormalPiece.WhiteKing, NormalPiece.BlackKing]))
            {
                Console.WriteLine();
                Console.Write($"{pin.PinnedPiece} on {pin.PinnedPieceSquare} is pinned against {pin.PinnedAgainstPiece} on {pin.PinnedAgainstSquare} " +
                     $"by the {pin.PinnedByPiece} on {pin.PinnedByPieceSquare} Absolute?: {pin.IsAbsolute}, Allowed Moves:");
                foreach (var square in pin.AllowedSquares)
                {
                    Console.Write($"{square}, ");
                }
                Console.WriteLine();
            }

            //List<BoardSquare> arrayOverlap = ListMethods.GetArrayOverlap(SquareRange.GetSquaresBetween(BoardSquare.A1, BoardSquare.H8, true), SquareRange.GetSquaresBetween(BoardSquare.G1, BoardSquare.A7, true));
            //NormalPieceClassifications.JumpingPieces.ToList().ForEach(s => Console.Write($"{s}, "));
            //constraintBuilder.SetStopCheck();
            List<GeneratedMove> kMoves = engineThreaded.MoveGenerator.GenerateAllLegalMoves(constraintBuilder.GetConstraintFunction());


            foreach (GeneratedMove move in kMoves)
            {
                Console.Write($"{move.FromSquare}-{move.DestSquare}, EP-square: {move.EnPassantSquare}, promotion?: {move.IsPromotion}, check?: {move.IsCheck}, capture?: {move.IsCapture}, attacked squares: ");
                //move.AttackedSquares.ForEach(seq => seq.ForEach(i => Console.Write($"({i.AsPiece}->{i.Square}:{i.PieceOnSquare}-{i.Interaction.ToShortString()}{(i.IsPromotion ? "++" : "")}) ")));
                Console.WriteLine();
            }
            Console.WriteLine();
            //SquareRange.GetSquaresBetween(BoardSquare.F5, BoardSquare.B1, Directions.Diagonal, true).ForEach(sq => Console.Write($"{sq}, "));
            //SquareRange.GetSquaresBetween(BoardSquare.B1, BoardSquare.F5, Directions.Diagonal, true).ForEach(sq => Console.Write($"{sq}, "));
            //^SAVE FOR LOGGING LATER^

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

        }
    }
}
//Console.WriteLine(-27.ConvertBoardIndexing(SquareIndexType.Base_120)); //the minus is considered after the conversion happens