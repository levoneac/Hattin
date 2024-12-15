namespace Hattin.Types
{
    public struct AttackProjection
    {
        public BoardSquare Square { get; }
        public NormalPiece PieceOnSquare { get; }
        public SquareInteraction Interaction { get; }
        public bool IsPromotion { get; }

        public AttackProjection(BoardSquare square, NormalPiece attackedPiece, SquareInteraction interaction, bool isPromotion)
        {
            Square = square;
            PieceOnSquare = attackedPiece;
            Interaction = interaction;
            IsPromotion = isPromotion;
        }

    }
}