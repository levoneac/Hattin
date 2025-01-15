using Hattin.Extensions.NormalPiece;
using Hattin.Utils;

namespace Hattin.Types
{
    public record GeneratedMove : Move, IComparable<GeneratedMove>
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

        public int CompareTo(GeneratedMove? obj)
        {
            if (obj is null && this is null) { return 0; }
            else if (this is null) { return 1; }
            else if (obj is null) { return -1; }

            int score = 0;

            if (IsCheck == true && obj.IsCheck == true) { return 0; }
            else if (IsCheck == true) { score -= 10; }
            else if (obj.IsCheck == true) { score += 10; }

            if (IsCapture == true && obj.IsCapture == true) { }
            else if (IsCapture == true)
            {
                if (Piece.ToValue() < obj.Piece.ToValue())
                {
                    score -= 10;
                }
                else
                {
                    score -= 1;
                }
            }
            else if (obj.IsCapture == true)
            {
                if (Piece.ToValue() > obj.Piece.ToValue())
                {
                    score += 10;
                }
                else
                {
                    score += 1;
                }
            }

            if (IsPromotion == true && obj.IsPromotion == true) { }
            else if (IsPromotion == true) { score -= 10; }
            else if (obj.IsPromotion == true) { score += 10; }

            return score;
        }
    }
}