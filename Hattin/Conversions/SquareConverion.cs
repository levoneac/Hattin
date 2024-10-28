using Hattin.Types;

namespace Hattin.Conversions
{
    public static class SquareConversions //struct?
    {
        public static int Convert(int index, SquareIndexType fromType, SquareIndexType toType)
        {
            if (fromType == SquareIndexType.Base_64 && toType == SquareIndexType.Base_120)
            {
                return Convert64to120(index);
            }
            else if (fromType == SquareIndexType.Base_120 && toType == SquareIndexType.Base_64)
            {
                return Convert120to64(index);
            }
            else
            {
                throw new NotImplementedException($"This conversion({fromType} to {toType}) is not yet implemented");
            }
        }

        public static int Convert64to120(int index)
        {
            if (index < 0 || index > 63)
            {
                throw new ArgumentOutOfRangeException($"{index} is out of range [0, 63]");
            }
            //base 120 starts at 21
            int start = 21;

            //each row has 8 columns 
            int column = index % 8;

            //integer division will give us the row number
            int row = index / 8;

            return start + (row * 10) + column;

        }

        public static int Convert120to64(int index)
        {
            if (index < 0 || index > 119)
            {
                throw new ArgumentOutOfRangeException($"{index} is out of range [0, 119]");
            }
            if (index < 21 || index > 98)
            {
                return (int)BoardSquares.NoSquare;
            }

            //Makes the base 0
            int start = index - 21;

            //numbers ending with 8 and 9 are outside the board
            int m = start % 10;
            if (m > 7)
            {
                return (int)BoardSquares.NoSquare;
            }

            //subtracts the 2 missing out of bounds squares for each row
            //casting the floor to int is ok here as the index is bounded within integer limits
            return start - (int)Math.Floor((float)(start / 10)) * 2;
        }

    }
}