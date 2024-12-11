namespace Hattin.Extensions.SideToMove
{
    public static class SideToMoveExtensions
    {
        public static Types.SideToMove RemoveWhite(this Types.SideToMove curColor)
        {
            if(curColor == Types.SideToMove.None || curColor == Types.SideToMove.White){
                return Types.SideToMove.None;
            }
            return Types.SideToMove.Black;
        }

        public static Types.SideToMove RemoveBlack(this Types.SideToMove curColor)
        {
            if(curColor == Types.SideToMove.None || curColor == Types.SideToMove.Black){
                return Types.SideToMove.None;
            }
            return Types.SideToMove.White;
        }

        public static Types.SideToMove RemoveBoth(this Types.SideToMove curColor)
        {
            return Types.SideToMove.None;
        }
    }
}