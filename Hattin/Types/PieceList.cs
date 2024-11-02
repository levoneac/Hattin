namespace Hattin.Types
{
    //WIP, need to see usecase first
    public class PieceList<T> where T : Enum
    {
        public List<BoardSquare>[] PiecePositions { get; set; }
        public int NumPieces { get; private set; }
        public PieceList()
        {
            NumPieces = Enum.GetNames(typeof(T)).Length;
            PiecePositions = new List<BoardSquare>[NumPieces];
            for (int i = 0; i < NumPieces; i++)
            {
                PiecePositions[i] = new List<BoardSquare>();
            }
        }

        public void ClearPieceList()
        {
            foreach (List<BoardSquare> listOfPieces in PiecePositions)
            {
                listOfPieces.Clear();
            }
        }
    }
}