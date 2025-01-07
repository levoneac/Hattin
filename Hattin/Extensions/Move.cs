using Hattin.Types;

namespace Hattin.Extensions
{
    public static class MoveExtensions
    {
        public static string ToAlgebra(this Move move)
        {
            string promoteTo = "";
            if (move.PromoteTo != Types.NormalPiece.Empty)
            {
                promoteTo = ((FENSymbols)move.PromoteTo).ToString().ToLower();
            }
            return $"{move.FromSquare.ToString().ToLower()}{move.DestSquare.ToString().ToLower()}{promoteTo}";
        }
    }
}