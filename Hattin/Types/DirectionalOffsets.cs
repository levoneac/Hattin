namespace Hattin.Types
{
    public enum DirectionalOffsets : int
    {
        ColumnRight = 1,
        ColumnLeft = -1,
        RowUp = 10,
        Rowdown = -10,
        Upleft = 9,
        UpRight = 11,
        Downleft = -11,
        Downright = -9
    }

    //From whites perspective
    public enum AbsoluteDirectionalOffsets : int
    {
        Column = 1,
        Row = 10,
        DiagonalRight = 11,
        DiagonalLeft = 9
    }
}