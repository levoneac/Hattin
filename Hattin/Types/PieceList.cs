namespace Hattin.Types
{
    //WIP, need to see usecase first
    public class PieceList<T> where T : Enum
    {
        public List<List<int>> Pieces { get; set; }
        public PieceList()
        {
            Pieces = new List<List<int>>();

        }
    }
}