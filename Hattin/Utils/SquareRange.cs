using System.Dynamic;
using Hattin.Types;

namespace Hattin.Utils
{
    public static class SquareRange
    {
        public static List<BoardSquare> GetSquaresBetween(BoardSquare fromSquare, BoardSquare toSquare, Directions direction, bool inclusive)
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
                case Directions.Row:
                    return GetRowBetween(fromSquare, toSquare, inclusive);

                case Directions.Column:
                    return GetColumnBetween(fromSquare, toSquare, inclusive);

                case Directions.Diagonal:
                    return GetDiagonalBetween(fromSquare, toSquare, inclusive);
            }
            return new List<BoardSquare>();
        }

        private static List<BoardSquare> GetDiagonalBetween(BoardSquare fromSquare, BoardSquare toSquare, bool inclusive)
        {
            List<BoardSquare> squares = new List<BoardSquare>();

            //Lower as in lower in a range from A1 to H8
            BoardSquare highestSquare = (BoardSquare)Math.Max((int)fromSquare, (int)toSquare);
            BoardSquare lowestSquare = (BoardSquare)Math.Min((int)fromSquare, (int)toSquare);

            string fromSquareName = Enum.GetName(typeof(BoardSquare), lowestSquare) ?? throw new ArgumentException($"fromSquare is invalid", nameof(fromSquare));
            string toSquareName = Enum.GetName(typeof(BoardSquare), highestSquare) ?? throw new ArgumentException($"toSquare is invalid", nameof(toSquare));

            AbsoluteDirectionalOffsets offset;
            if (fromSquareName[0] > toSquareName[0]) { offset = AbsoluteDirectionalOffsets.DiagonalLeft; }
            else if (fromSquareName[0] < toSquareName[0]) { offset = AbsoluteDirectionalOffsets.DiagonalRight; }
            else { throw new ArgumentException($"the squares cannot be on the same Column", nameof(fromSquare)); }

            int yDirection;
            if (fromSquareName[1] > toSquareName[1]) { yDirection = 1; }
            else if (fromSquareName[1] < toSquareName[1]) { yDirection = 1; }
            else { throw new ArgumentException($"the squares cannot be on the same Row", nameof(fromSquare)); }

            int numSquares = Math.Abs(fromSquareName[1] - toSquareName[1]) - 1;
            BoardSquare curSquare = lowestSquare + ((int)offset) * yDirection;

            for (int i = 0; i < numSquares; i++)
            {
                squares.Add(curSquare);
                curSquare += ((int)offset) * yDirection;
            }
            if (curSquare != highestSquare) { throw new ArgumentException($"The squares are not on the same diagonal", nameof(fromSquare)); }

            if (inclusive)
            {
                squares.Add(lowestSquare);
                squares.Add(highestSquare);
            }
            return squares;
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