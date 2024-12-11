using Hattin.Types;

namespace Hattin.Extensions.Squares
{
    public static class SquareExtensions
    {
        public static int ToBase64Int(this BoardSquare square)
        {
            return Conversions.SquareConversions.Array120To64[(int)square];
        }

        public static int ToBase120Int(this BoardSquare square)
        {
            return (int)square;
        }

        //converts between 64 and 120 indexing
        public static int ConvertBoardIndexing(this int integer, SquareIndexType boardType)
        {
            return Conversions.SquareConversions.AutoChooseConvert64And120(integer, boardType);
        }
    }
}