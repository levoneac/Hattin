using Hattin.Extensions.NormalPiece;
using Hattin.Utils;

namespace Hattin.Types
{
    public record GeneratedMove : Move
    {
        public List<List<AttackProjection>> AttackedSquares { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCapture { get; set; }
        public bool IsCheck { get; set; }

        public GeneratedMove() : base()
        {
            AttackedSquares = [];
            IsPromotion = false;
            IsCapture = false;
            IsCheck = false;
        }

        public GeneratedMove(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare, List<List<AttackProjection>> attackedSquares,
            BoardSquare enPassantSquare = BoardSquare.NoSquare, BoardSquare enPassantCaptureSquare = BoardSquare.NoSquare, bool isPromotion = false,
            bool isCapture = false, BoardSquare rookCastleFromSquare = BoardSquare.NoSquare, BoardSquare rookCastleToSquare = BoardSquare.NoSquare)
                : base(piece, fromSquare, toSquare, rookCastleFromSquare, rookCastleToSquare, enPassantSquare: enPassantSquare, enPassantCaptureSquare: enPassantCaptureSquare)
        {
            AttackedSquares = attackedSquares;
            EnPassantSquare = enPassantSquare;
            IsPromotion = isPromotion;
            IsCapture = isCapture;

            NormalPiece opponentKingColor = piece.ToColor() == SideToMove.White ? NormalPiece.BlackKing : NormalPiece.WhiteKing;

            //maybe move out into movegenerator for more efficiency
            foreach (var attackDirection in attackedSquares)
            {
                AttackProjection kingAttack = attackDirection.FirstOrDefault(sq => sq.PieceOnSquare == opponentKingColor, new AttackProjection());
                if (kingAttack.AsPiece != NormalPiece.Empty && kingAttack.XRayLevel == 0)
                {
                    IsCheck = true;
                    break;
                }
            }
        }
    }
}