using Hattin.Types;

namespace Hattin.Extensions.Move
{
    public static class MoveExtensions
    {
        public static string ToAlgebra(this Types.Move move)
        {
            return Types.Move.ToAlgebra(move);
        }

        //Is it bad practise to overload builtins?
        public static Types.Move FromAlgebra(this string move, BoardState board)
        {
            return Types.Move.GetMoveFromAlgebra(move, board);
        }
    }
}