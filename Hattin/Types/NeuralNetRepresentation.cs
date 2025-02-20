namespace Hattin.Types
{
    public class NeuralNetRepresentation
    {
        public int[] NeuralInput;
        public NeuralNetRepresentation()
        {
            NeuralInput = new int[772]; //inits all numbers to 0 by default
        }
        //This class should probably have had its own MovePiece function, but then i would need to do all the move checks another time
        //Im not sure the cost of doing that is worth it 

        public void Clear()
        {
            Array.Clear(NeuralInput);
        }

        public void SetValue(NormalPiece piece, BoardSquare square, bool exists)
        {
            if (piece == NormalPiece.Empty || square == BoardSquare.NoSquare)
            {
                throw new ArgumentException($"Piece cant be empty ({piece} and square cant be nosquare ({square}))");
            }
            int arrPos = (int)piece * ((int)square - 21 + 1);
            if (exists)
            {
                NeuralInput[arrPos] = 1;
            }
            else
            {
                NeuralInput[arrPos] = 0;
            }
        }

        //not sure if this is even worth it
        public void SetValue(CastleRights castleRights)
        {
            if (castleRights.HasFlag(CastleRights.WhiteKingsideCastle))
            {
                NeuralInput[768] = 1;
            }
            else
            {
                NeuralInput[768] = 0;
            }

            if (castleRights.HasFlag(CastleRights.WhiteQueensideCastle))
            {
                NeuralInput[769] = 1;
            }
            else
            {
                NeuralInput[769] = 0;
            }

            if (castleRights.HasFlag(CastleRights.BlackKingsideCastle))
            {
                NeuralInput[770] = 1;
            }
            else
            {
                NeuralInput[770] = 0;
            }

            if (castleRights.HasFlag(CastleRights.BlackQueensideCastle))
            {
                NeuralInput[771] = 1;
            }
            else
            {
                NeuralInput[771] = 0;
            }
        }
    }
}