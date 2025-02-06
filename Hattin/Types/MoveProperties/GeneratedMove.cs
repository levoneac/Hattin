using System.ComponentModel.DataAnnotations;
using Hattin.Extensions.NormalPiece;
using Hattin.Utils;

namespace Hattin.Types
{
    public class GeneratedMove : Move, IComparable<GeneratedMove>, IEquatable<GeneratedMove>
    {
        public List<List<AttackProjection>> AttackedSquares { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCapture { get; set; }
        public bool IsEnPassant { get; set; }
        public bool IsCheck { get; set; }

        public GeneratedMove() : base()
        {
            AttackedSquares = [];
            IsPromotion = false;
            IsCapture = false;
            IsEnPassant = false;
            IsCheck = false;
        }

        public GeneratedMove(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare, List<List<AttackProjection>> attackedSquares,
            BoardSquare enPassantSquare = BoardSquare.NoSquare, BoardSquare enPassantCaptureSquare = BoardSquare.NoSquare, bool isPromotion = false, NormalPiece promoteTo = NormalPiece.Empty,
            bool isCapture = false, bool isEnPassant = false, bool isCheck = false, BoardSquare rookCastleFromSquare = BoardSquare.NoSquare, BoardSquare rookCastleToSquare = BoardSquare.NoSquare)
                : base(piece, fromSquare, toSquare, rookCastleFromSquare, rookCastleToSquare, promoteTo: promoteTo, enPassantSquare: enPassantSquare, enPassantCaptureSquare: enPassantCaptureSquare)
        {
            AttackedSquares = attackedSquares;
            IsPromotion = isPromotion;
            IsEnPassant = isEnPassant;
            IsCapture = isCapture;
            IsCheck = isCheck;


            if (!IsCheck)
            {
                NormalPiece opponentKingColor = piece.ToColor() == SideToMove.White ? NormalPiece.BlackKing : NormalPiece.WhiteKing;
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

        //Use method in Engine instead, as this lacks access to the attack array
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
        public bool Equals(GeneratedMove? other)
        {
            if (this is null) { return false; }
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            if (Piece != other.Piece) { return false; }
            if (FromSquare != other.FromSquare) { return false; }
            if (DestSquare != other.DestSquare) { return false; }
            if (PromoteTo != other.PromoteTo) { return false; }
            if (RookCastleFromSquare != other.RookCastleFromSquare) { return false; }
            if (RookCastleToSquare != other.RookCastleToSquare) { return false; }
            if (EnPassantSquare != other.EnPassantSquare) { return false; }
            if (EnPassantCaptureSquare != other.EnPassantCaptureSquare) { return false; }

            return true;
        }
    }
}