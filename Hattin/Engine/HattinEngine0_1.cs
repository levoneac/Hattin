using Hattin.Interfaces;

namespace Hattin.Engine
{
    public class HattinEngine0_1
    {
        public IBoard Board { get; private set; }
        public IMoveGenerator MoveGenerator { get; private set; }
        public IPositionEvaluator PositionEvaluator { get; private set; }
        public HattinEngine0_1(IBoard board, IMoveGenerator moveGenerator, IPositionEvaluator positionEvaluator)
        {
            Board = board;
            MoveGenerator = moveGenerator;
            PositionEvaluator = positionEvaluator;
        }
    }
}