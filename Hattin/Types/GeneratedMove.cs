using Hattin.Extensions.NormalPiece;

namespace Hattin.Types
{
    public class GeneratedMove : Move
    {
        public List<AttackProjection> AttackedSquares { get; set; }
        public BoardSquare EnPassantSquare { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCapture { get; set; }
        public bool IsCheck { get; set; }//be aware of edgecases from promotion
        public List<BoardSquare> CheckPath { get; set; }
        //isMate?
        //isBlocking??
        //

        public GeneratedMove() : base()
        {
            AttackedSquares = [];
            EnPassantSquare = BoardSquare.NoSquare;
            IsPromotion = false;
            IsCapture = false;
            IsCheck = false;
        }

        public GeneratedMove(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare, List<AttackProjection> attackedSquares, BoardSquare enpassantSquare = BoardSquare.NoSquare, bool isPromotion = false, bool isCapture = false, BoardSquare rookCastleSquare = BoardSquare.NoSquare)
        : base(piece, fromSquare, toSquare, rookCastleSquare)
        {
            AttackedSquares = attackedSquares;
            EnPassantSquare = enpassantSquare;
            IsPromotion = isPromotion;
            IsCapture = isCapture;
            CheckPath = new List<BoardSquare>();

            NormalPiece opponentKingColor = piece.ToColor() == SideToMove.White ? NormalPiece.BlackKing : NormalPiece.WhiteKing;

            var checks = attackedSquares.Select(i => i.PieceOnSquare == opponentKingColor);
        }
    }
}