namespace Hattin.Types
{
    public enum SideToMove : int
    {
        Black = -1,
        None,
        White,
        Both //Subtracting one color gives the other
    }
}