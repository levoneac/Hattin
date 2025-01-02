using Hattin.Extensions.NormalPiece;

namespace Hattin.Types
{
    public readonly struct AttackProjection
    {
        public NormalPiece AsPiece { get; }
        public SideToMove AsSide { get; }
        public BoardSquare Square { get; }
        public NormalPiece PieceOnSquare { get; }
        public SquareInteraction Interaction { get; }
        public int MoveSequence { get; }
        public bool IsPromotion { get; }
        public int XRayLevel { get; }

        public AttackProjection()
        {
            AsPiece = NormalPiece.Empty;
            AsSide = SideToMove.None;
            Square = BoardSquare.NoSquare;
            PieceOnSquare = NormalPiece.Empty;
            Interaction = SquareInteraction.OwnSquare;
            MoveSequence = -1;
            IsPromotion = false;
            XRayLevel = -1;
        }
        public AttackProjection(NormalPiece asPiece, BoardSquare square, NormalPiece attackedPiece, SquareInteraction interaction, int moveSequence, bool isPromotion, int xRayLevel = 0)
        {
            AsPiece = asPiece;
            AsSide = asPiece.ToColor();
            Square = square;
            PieceOnSquare = attackedPiece;
            Interaction = interaction;
            MoveSequence = moveSequence;
            IsPromotion = isPromotion;
            XRayLevel = xRayLevel;
        }

    }
}