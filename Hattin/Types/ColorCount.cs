namespace Hattin.Types
{
    public class ColorCount
    {
        public int White = 0;
        public int Black = 0;


        public void IncrementColor(SideToMove color)
        {
            if (color == SideToMove.White)
            {
                White++;
            }
            else if (color == SideToMove.Black)
            {
                Black++;
            }
            else
            {
                throw new ArgumentException($"Only White or Black is allowed as an argument", nameof(color));
            }
        }
    }
}