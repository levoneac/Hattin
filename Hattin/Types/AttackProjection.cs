using Hattin.Extensions.NormalPiece;

namespace Hattin.Types
{
    public readonly struct AttackProjection
    {
        public NormalPiece AsPiece { get; }
        public SideToMove AsSide {get ;}
        public BoardSquare Square { get; }
        public NormalPiece PieceOnSquare { get; }
        public SquareInteraction Interaction { get; }
        public bool IsPromotion { get; }

        public AttackProjection(NormalPiece asPiece, BoardSquare square, NormalPiece attackedPiece, SquareInteraction interaction, bool isPromotion)
        {
            AsPiece = asPiece;
            AsSide = asPiece.ToColor();
            Square = square;
            PieceOnSquare = attackedPiece;
            Interaction = interaction;
            IsPromotion = isPromotion;
        }

    }
}