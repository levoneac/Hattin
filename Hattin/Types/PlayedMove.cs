namespace Hattin.Types
{
    //Saved in moveHistory
    public record PlayedMove
    {
        //BOARDSTATE DIRECT
        public CastleRights CastleRights { get; init; }
        public BoardSquare EnPassantSquare { get; init; }
        public BoardSquare EnPassantCaptureSquare { get; init; } //spawn new pawn on this square
        public int PlyCounter { get; init; } //just --
        public int PliesWithoutCapture { get; init; }
        public SideToMove SideToMove { get; init; }


        //MOVE PROPERTIES FOR UPDATING
        public NormalPiece PromotedFromPiece { get; init; }
        public NormalPiece PromotedToPiece { get; init; }
        public BoardSquare FromSquare { get; init; }
        public BoardSquare DestSquare { get; init; }
        public BoardSquare RookSourceSquare { get; init; }
        public BoardSquare RookDestSquare { get; init; }


        //ctor
    }
}