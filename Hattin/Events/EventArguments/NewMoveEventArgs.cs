using Hattin.Types;

namespace Hattin.Events.EventArguments
{
    public class NewMoveEventArgs : EventArgs
    {
        public NormalPiece Piece { get; private set; }
        public BoardSquare FromSquare { get; private set; }
        public BoardSquare DestSquare { get; private set; }
        public NormalPiece PromoteTo { get; private set; }

        public NewMoveEventArgs(Move move)
        {
            Piece = move.Piece;
            FromSquare = move.FromSquare;
            DestSquare = move.DestSquare;
            PromoteTo = move.PromoteTo;
        }
        public NewMoveEventArgs(NormalPiece piece, BoardSquare fromSquare, BoardSquare destSquare, NormalPiece promoteTo)
        {
            Piece = piece;
            FromSquare = fromSquare;
            DestSquare = destSquare;
            PromoteTo = promoteTo;
        }
    }
}