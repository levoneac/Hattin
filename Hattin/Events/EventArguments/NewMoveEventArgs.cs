using Hattin.Types;

namespace Hattin.Events.EventArguments
{
    public class NewMoveEventArgs : EventArgs
    {
        public NormalPiece Piece { get; private set; }
        public BoardSquare FromSquare { get; private set; }
        public BoardSquare DestSquare { get; private set; }
        public NormalPiece PromoteTo { get; private set; }
        public BoardSquare RookCastleFromSquare { get; private set; }
        public BoardSquare RookCastleToSquare { get; private set; }
        public BoardSquare EnPassantSquare { get; private set; }
        public BoardSquare EnPassantCaptureSquare { get; private set; }

        public NewMoveEventArgs(Move move)
        {
            Piece = move.Piece;
            FromSquare = move.FromSquare;
            DestSquare = move.DestSquare;
            PromoteTo = move.PromoteTo;
            RookCastleFromSquare = move.RookCastleFromSquare;
            RookCastleToSquare = move.RookCastleToSquare;
            EnPassantSquare = move.EnPassantSquare;
            EnPassantCaptureSquare = move.EnPassantCaptureSquare;
        }
    }
}