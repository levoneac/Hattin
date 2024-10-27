namespace Hattin.Types
{
    [Flags] //tells that the enum should be treated as bit fields
    public enum CastleRights
    {
        WhiteKingsideCastle = 0b_0001,
        WhiteQueensideCastle = 0b_0010,
        BlackKingsideCastle = 0b_0100,
        BlackQueensideCastle = 0b_1000
    }
}