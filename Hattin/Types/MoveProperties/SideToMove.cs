namespace Hattin.Types
{
    public enum SideToMove : int
    {
        None = 0, 
        White,
        Black,
        Both = White + Black //Subtracting one color gives the other
    }
}