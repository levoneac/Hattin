namespace Hattin.Types
{
    public class GeneratedMove : Move
    {
        public List<AttackProjection> AttackedSquares { get; set; }
        public BoardSquare EnPassantSquare { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCapture { get; set; }
        //isCheck
        //isMate?
        //isBlocking??
        //

        public GeneratedMove() : base()
        {
            AttackedSquares = [];
            EnPassantSquare = BoardSquare.NoSquare;
            IsPromotion = false;
            IsCapture = false;
        }

        public GeneratedMove(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare, List<AttackProjection> attackedSquares, BoardSquare enpassantSquare, bool isPromotion, bool isCapture)
        : base(piece, fromSquare, toSquare)
        {
            AttackedSquares = attackedSquares;
            EnPassantSquare = enpassantSquare;
            IsPromotion = isPromotion;
            IsCapture = isCapture;
        }
    }
}