using Hattin.Types;

namespace Hattin.Extensions
{
    public static class MoveExtensions
    {
        public static string ToAlgebra(this Move move)
        {
            return $"{move.FromSquare.ToString().ToLower()}{move.DestSquare.ToString().ToLower()}";
            //Add promotion
        }
    }
}