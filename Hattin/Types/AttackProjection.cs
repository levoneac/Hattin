namespace Hattin.Types
{
    public struct AttackProjection
    {
        public BoardSquare Square { get; set; }
        public NormalPiece PieceOnSquare { get; set; }

        public AttackProjection(BoardSquare square, NormalPiece attackedPiece)
        {
            Square = square;
            PieceOnSquare = attackedPiece;
        }

    }
}