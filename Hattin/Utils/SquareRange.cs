using System.Dynamic;
using Hattin.Types;

namespace Hattin.Utils
{
    public static class SquareRange
    {
        public static List<BoardSquare> GetSquaresBetween(BoardSquare fromSquare, BoardSquare toSquare, AbsoluteDirectionalOffsets direction, bool inclusive)
        {
            if (fromSquare == BoardSquare.NoSquare)
            {
                throw new ArgumentException($"Nosquare not allowed", nameof(fromSquare));
            }
            if (toSquare == BoardSquare.NoSquare)
            {
                throw new ArgumentException($"Nosquare not allowed", nameof(toSquare));
            }

            switch (direction)
            {
                case AbsoluteDirectionalOffsets.Row:
                    return GetRowBetween(fromSquare, toSquare, inclusive);

                case AbsoluteDirectionalOffsets.Column:
                    return GetColumnBetween(fromSquare, toSquare, inclusive);

            }
            return new List<BoardSquare>();
        }

        //Minor issue: GetRow is always ascending while GetColumn goes from fromSquare to toSquare as the user specified
        private static List<BoardSquare> GetRowBetween(BoardSquare fromSquare, BoardSquare toSquare, bool inclusive)
        {
            List<BoardSquare> squares = new List<BoardSquare>();

            int diff = Math.Abs(fromSquare - toSquare);
            if (diff % (int)AbsoluteDirectionalOffsets.Row != 0)
            {
                throw new ArgumentException($"{fromSquare} and {toSquare} is not on the same Column", nameof(fromSquare));
            }
            int numRows = diff / (int)AbsoluteDirectionalOffsets.Row;
            int lowest = Math.Min((int)fromSquare, (int)toSquare);
            for (int row = 1; row < numRows; row++)
            {
                squares.Add((BoardSquare)(lowest + (row * (int)AbsoluteDirectionalOffsets.Row)));
            }

            if (inclusive)
            {
                squares.Insert(0, fromSquare);
                squares.Add(toSquare);
            }
            return squares;
        }

        private static List<BoardSquare> GetColumnBetween(BoardSquare fromSquare, BoardSquare toSquare, bool inclusive)
        {
            List<BoardSquare> squares = new List<BoardSquare>();

            //Ensure that the squares are on the same row
            char fromRow = Enum.GetName(typeof(BoardSquare), fromSquare)?[1] ?? throw new ArgumentException($"fromSquare is invalid", nameof(fromSquare));
            char toRow = Enum.GetName(typeof(BoardSquare), toSquare)?[1] ?? throw new ArgumentException($"toSquare is invalid", nameof(toSquare));
            if (!(fromRow == toRow))
            {
                throw new ArgumentException($"{fromSquare} and {toSquare} is not on the same Row", nameof(fromSquare));
            }

            //Wish you could get both from one call
            int lowest = Math.Min((int)fromSquare, (int)toSquare) + 1;
            int highest = Math.Max((int)fromSquare, (int)toSquare);
            while (lowest < highest)
            {
                squares.Add((BoardSquare)lowest);
                lowest++;
            }

            if (inclusive)
            {
                squares.Insert(0, fromSquare);
                squares.Add(toSquare);
            }
            return squares;
        }

        //TODO: Diagonals if needed
    }
}