namespace Hattin.Types
{
    public class GeneratedMove : Move
    {
        public BoardSquare EnPassantSquare { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCapture { get; set; }

        public GeneratedMove() : base()
        {
            EnPassantSquare = BoardSquare.NoSquare;
            IsPromotion = false;
            IsCapture = false;
        }

        public GeneratedMove(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare, BoardSquare enpassantSquare, bool isPromotion, bool isCapture)
        : base(piece, fromSquare, toSquare)
        {
            EnPassantSquare = enpassantSquare;
            IsPromotion = isPromotion;
            IsCapture = isCapture;
        }
    }
}