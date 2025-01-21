using Hattin.Implementations.Engine;
using Hattin.Extensions.SideToMove;
using Hattin.Extensions.SquareInteraction;
using Hattin.Extensions.Squares;
using Hattin.Implementations.Controllers;
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
            IMoveGenerator threadedGenerator = new BasicMoveGeneratorThreaded(board);
            HattinEngine0_1 engineThreaded = new HattinEngine0_1(board, threadedGenerator, constraintBuilder, evaluator);

            //UCIController controller = new UCIController(engineThreaded);
            //controller.StartListening();
            Perft perft = new Perft(engineThreaded);
            perft.PrintTotalMovesTillDepth(4, "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");  //8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1 enpassant capture reveals the king to a rook
            perft.PrintTotalMovesTillDepth(3, "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
            //Idea: dont update the attacked squares unless king is in check(how to know this without doning it? do the move and if the king is captured its check(what about castling?))


            //v SAVE FOR LOGGING LATER v

            //PIN INFORMATION
            //foreach (var pin in board.PieceProperties.GetPinnedPieces([NormalPiece.WhiteKing, NormalPiece.BlackKing]))
            //{
            //    Console.WriteLine();
            //    Console.Write($"{pin.PinnedPiece} on {pin.PinnedPieceSquare} is pinned against {pin.PinnedAgainstPiece} on {pin.PinnedAgainstSquare} " +
            //         $"by the {pin.PinnedByPiece} on {pin.PinnedByPieceSquare} Absolute?: {pin.IsAbsolute}, Allowed Moves:");
            //    foreach (var square in pin.AllowedSquares)
            //    {
            //        Console.Write($"{square}, ");
            //    }
            //    Console.WriteLine();
            //}

            //board.ProcessFEN("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2 ");

            //PIECE POSITIONS
            //for (int i = 0; i < board.PieceProperties.PiecePositions.Count; i++)
            //{
            //    Console.Write($"{(NormalPiece)i}: ");
            //    for (int j = 0; j < board.PieceProperties.PiecePositions[i].Count; j++)
            //    {
            //        Console.Write($"{board.PieceProperties.PiecePositions[i][j]}, ");
            //    }
            //    Console.WriteLine();
            //}
            //PieceTotals total = board.PieceProperties.CalculatePieceTotals();
            //Console.WriteLine("white: {0}, black: {1}", total.white, total.black);

            //CONSTRAINTBUILDER
            //constraintBuilder.SetPinRestriction();
            //List<GeneratedMove> kMoves = engineThreaded.MoveGenerator.GenerateAllLegalMoves(constraintBuilder.GetConstraintFunction());
            //
            //
            //foreach (GeneratedMove move in kMoves)
            //{
            //    Console.Write($"{move.FromSquare}-{move.DestSquare}, EP-square: {move.EnPassantSquare}, promotion?: {move.IsPromotion}, check?: {move.IsCheck}, capture?: {move.IsCapture}, attacked squares: ");
            //    //move.AttackedSquares.ForEach(seq => seq.ForEach(i => Console.Write($"({i.AsPiece}->{i.Square}:{i.PieceOnSquare}-{i.Interaction.ToShortString()}{(i.IsPromotion ? "++" : "")}) ")));
            //    Console.WriteLine();
            //}

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