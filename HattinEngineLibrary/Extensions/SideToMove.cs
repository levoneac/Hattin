namespace HattinEngineLibrary.Extensions.SideToMove
{
    public static class SideToMoveExtensions
    {
        public static Types.SideToMove RemoveWhite(this Types.SideToMove curColor)
        {
            if (curColor == Types.SideToMove.None || curColor == Types.SideToMove.White)
            {
                return Types.SideToMove.None;
            }
            return Types.SideToMove.Black;
        }

        public static Types.SideToMove RemoveBlack(this Types.SideToMove curColor)
        {
            if (curColor == Types.SideToMove.None || curColor == Types.SideToMove.Black)
            {
                return Types.SideToMove.None;
            }
            return Types.SideToMove.White;
        }

        public static Types.SideToMove RemoveBoth(this Types.SideToMove curColor)
        {
            return Types.SideToMove.None;
        }

        public static Types.SideToMove ToOpposite(this Types.SideToMove curColor)
        {
            return curColor == Types.SideToMove.White ? Types.SideToMove.Black :
                        curColor == Types.SideToMove.Black ? Types.SideToMove.White :
                            curColor == Types.SideToMove.None ? Types.SideToMove.Both :
                                curColor == Types.SideToMove.Both ? Types.SideToMove.None : Types.SideToMove.None;
        }

        public static Types.SideToMove ToOppositeColor(this Types.SideToMove curColor)
        {
            return curColor == Types.SideToMove.White ? Types.SideToMove.Black :
                        curColor == Types.SideToMove.Black ? Types.SideToMove.White : throw new ArgumentException($"SideToMove.{curColor} is not either Black or White", nameof(curColor));
        }
    }
}