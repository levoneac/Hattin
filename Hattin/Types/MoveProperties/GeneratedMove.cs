using Hattin.Extensions.NormalPiece;
using Hattin.Utils;

namespace Hattin.Types
{
    public class GeneratedMove : Move
    {
        public List<List<AttackProjection>> AttackedSquares { get; set; }
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
            IsPromotion = false;
            IsCapture = false;
            IsCheck = false;
            CheckPath = [];
        }

        public GeneratedMove(NormalPiece piece, BoardSquare fromSquare, BoardSquare toSquare, List<List<AttackProjection>> attackedSquares,
            BoardSquare enPassantSquare = BoardSquare.NoSquare, BoardSquare enPassantCaptureSquare = BoardSquare.NoSquare, bool isPromotion = false,
            bool isCapture = false, BoardSquare rookCastleSquare = BoardSquare.NoSquare)
                : base(piece, fromSquare, toSquare, rookCastleSquare, enPassantSquare: enPassantSquare, enPassantCaptureSquare: enPassantCaptureSquare)
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
                    //CheckPath = SquareRange.GetSquaresBetween(fromSquare, kingAttack.Square, true);
                    break;
                }
            }
            CheckPath ??= new List<BoardSquare>();

            //var checks = attackedSquares.Select(i => i.PieceOnSquare == opponentKingColor);
        }
    }
}