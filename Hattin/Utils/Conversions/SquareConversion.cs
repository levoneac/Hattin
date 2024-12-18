using Hattin.Types;

namespace Hattin.Utils.Conversions
{
    public static class SquareConversions 
    {
        //Less safe to use these directly, but probably a bit faster
        public static readonly int[] Array64To120 = new int[64];
        public static readonly int[] Array120To64 = new int[120];
        static SquareConversions() //static constructors run only once
        {
            for (int i = 0; i < 64; i++)
            {
                Array64To120[i] = Convert64To120(i);
                Array120To64[i] = Convert120To64(i);
            }
            for (int i = 64; i < 120; i++)
            {
                Array120To64[i] = Convert120To64(i);
            }
        }
        public static int Convert(int index, SquareIndexType fromType, SquareIndexType toType)
        {
            if (fromType == SquareIndexType.Base_64 && toType == SquareIndexType.Base_120)
            {
                return Convert64To120(index);
            }
            else if (fromType == SquareIndexType.Base_120 && toType == SquareIndexType.Base_64)
            {
                return Convert120To64(index);
            }
            else
            {
                throw new NotSupportedException($"This conversion({fromType} to {toType}) is not yet implemented");
            }
        }
        public static int AutoChooseConvert64And120(int index, SquareIndexType fromType)
        {
            if (fromType == SquareIndexType.Base_64)
            {
                return Convert64To120(index);
            }
            else if (fromType == SquareIndexType.Base_120)
            {
                return Convert120To64(index);
            }
            else
            {
                throw new NotImplementedException("Only base 64 and 120 allowed");
            }
        }

        public static int Convert64To120(int index)
        {
            if (index < 0 || index > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is out of range [0, 63]");
            }
            //base 120 starts at 21
            int start = 21;

            //each row has 8 columns 
            int column = index % 8;

            //integer division will give us the row number
            int row = index / 8;

            return start + (row * 10) + column;

        }

        public static int Convert120To64(int index)
        {
            if (index < 0 || index > 119)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is out of range [0, 119]");
            }
            if (index < 21 || index > 98)
            {
                return (int)BoardSquare.NoSquare;
            }

            //Makes the base 0
            int start = index - 21;

            //numbers ending with 8 and 9 are outside the board
            int m = start % 10;
            if (m > 7)
            {
                return (int)BoardSquare.NoSquare;
            }

            //subtracts the 2 missing out of bounds squares for each row
            //casting the floor to int is ok here as the index is bounded within integer limits
            return start - (int)Math.Floor((float)(start / 10)) * 2;
        }

    }
}