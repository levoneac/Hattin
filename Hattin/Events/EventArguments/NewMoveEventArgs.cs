using Hattin.Types;

namespace Hattin.Events.EventArguments
{
    public class NewMoveEventArgs : EventArgs
    {
        public NormalPiece Piece { get; private set; }
        public BoardSquare FromSquare { get; private set; }
        public BoardSquare ToSquare { get; private set; }

        public NewMoveEventArgs(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare)
        {
            Piece = piece;
            FromSquare = fromSquare;
            ToSquare = toSquare;
        }
    }
}