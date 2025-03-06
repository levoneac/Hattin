using HattinEngineLibrary.Types;

namespace HattinEngineLibrary.Utils
{
    public static class SquareRange
    {
        //Lists CAN be empty
        public static List<BoardSquare> GetSquaresBetween(BoardSquare fromSquare, BoardSquare toSquare, bool inclusive, Directions direction = Directions.Auto)
        {
            if (direction == Directions.Auto) { direction = InferDirection(fromSquare, toSquare); }
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

                case Directions.Knight:
                    return new List<BoardSquare>([fromSquare, toSquare]);
            }
            return new List<BoardSquare>();
        }

        public static Directions InferDirection(BoardSquare fromSquare, BoardSquare toSquare)
        {
            string fromSquareName = Enum.GetName(typeof(BoardSquare), fromSquare) ?? throw new ArgumentException($"({fromSquare}) is invalid for fromSquare", nameof(fromSquare));
            string toSquareName = Enum.GetName(typeof(BoardSquare), toSquare) ?? throw new ArgumentException($"({toSquare}) is invalid for toSquare", nameof(toSquare));

            if (char.Equals(fromSquareName[0], toSquareName[0])) { return Directions.Row; }
            if (char.Equals(fromSquareName[1], toSquareName[1])) { return Directions.Column; }
            if (IsKnightMove(fromSquare, toSquare)) { return Directions.Knight; }
            //if its not any of these we hope its diagonal (throws appropriate error if its not)
            return Directions.Diagonal;
        }

        private static bool IsKnightMove(BoardSquare fromSquare, BoardSquare toSquare)
        {
            return NormalPieceOffsets.Knight.Contains(fromSquare - toSquare);
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
            else { throw new ArgumentException($"the squares {lowestSquare} and {highestSquare} cannot be on the same Column", nameof(fromSquare)); }

            int yDirection;
            if (fromSquareName[1] > toSquareName[1]) { yDirection = 1; }
            else if (fromSquareName[1] < toSquareName[1]) { yDirection = 1; }
            else { throw new ArgumentException($"the squares {lowestSquare} and {highestSquare} cannot be on the same Row", nameof(fromSquare)); }

            int numSquares = Math.Abs(fromSquareName[1] - toSquareName[1]) - 1;
            BoardSquare curSquare = lowestSquare + ((int)offset) * yDirection;

            for (int i = 0; i < numSquares; i++)
            {
                squares.Add(curSquare);
                curSquare += ((int)offset) * yDirection;
            }
            if (curSquare != highestSquare) { throw new ArgumentException($"The squares {fromSquare} and {toSquare} are not on the same Diagonal", nameof(fromSquare)); }

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
        //Doesnt quite fit here
        public static BoardSquare GetEnPassantCaptureSquare(BoardSquare pawnFromSquare, BoardSquare pawnToSquare)
        {
            if (pawnFromSquare == BoardSquare.NoSquare || pawnToSquare == BoardSquare.NoSquare) { throw new ArgumentException($"Squares have to be on the board", nameof(pawnFromSquare)); }
            char enPassantRow = Enum.GetName(typeof(BoardSquare), pawnFromSquare)?[1] ?? throw new ArgumentException($"fromSquare is invalid", nameof(pawnFromSquare));
            char enpassantColumn = Enum.GetName(typeof(BoardSquare), pawnToSquare)?[0] ?? throw new ArgumentException($"toSquare is invalid", nameof(pawnToSquare));

            //frankensteinian
            string enPassantSquareString = enpassantColumn.ToString() + enPassantRow.ToString();

            if (Enum.TryParse(typeof(BoardSquare), enPassantSquareString, true, out object? result))
            {
                return (BoardSquare)result;
            }
            throw new Exception($"No square was found");
        }


        public static bool IsSquareRangeEmpty(List<BoardSquare> range, BoardState currentBoard)
        {
            foreach (BoardSquare square in range)
            {
                if (currentBoard.PieceProperties.GetPieceOnSquare(square) != NormalPiece.Empty)
                {
                    return false;
                }
            }
            return true;
        }
    }
}