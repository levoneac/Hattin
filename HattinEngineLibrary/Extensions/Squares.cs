using HattinEngineLibrary.Types;

namespace HattinEngineLibrary.Extensions.Squares
{
    public static class SquareExtensions
    {
        public static int ToBase64Int(this BoardSquare square)
        {
            return Utils.Conversions.SquareConversions.Array120To64[(int)square];
        }

        public static int ToBase120Int(this BoardSquare square)
        {
            return (int)square;
        }

        //converts between 64 and 120 indexing
        public static int ConvertBoardIndexing(this int integer, SquareIndexType boardType)
        {
            return Utils.Conversions.SquareConversions.AutoChooseConvert64And120(integer, boardType);
        }

        public static int ToFileEnumValue(this BoardSquare square)
        {
            if (square == BoardSquare.NoSquare)
            {
                throw new ArgumentException($"Nosquare doesnt have a file", nameof(square));
            }
            return 15 + (((int)square) % 10); //beware of changing the enums with this

            //Safe and slow solution
            //string squareString = Enum.GetName(typeof(BoardSquare), square) ?? throw new Exception($"{square} Square not found");
            //if (Enum.TryParse(typeof(BoardFile), squareString[0].ToString(), out object? file))
            //{
            //    return (int)(BoardFile)file;
            //}
            //throw new Exception($"File not found for {square}");
        }
    }
}