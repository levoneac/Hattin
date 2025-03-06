using HattinEngineLibrary.Types;

namespace HattinEngineLibrary.Extensions.Move
{
    public static class MoveExtensions
    {
        public static string ToAlgebra(this Types.Move move, bool includePiece = false)
        {
            return Types.Move.ToAlgebra(move, includePiece);
        }

        //Is it bad practise to overload builtins?
        public static Types.Move FromAlgebra(this string move, BoardState board)
        {
            return Types.Move.GetMoveFromAlgebra(move, board);
        }
    }
}