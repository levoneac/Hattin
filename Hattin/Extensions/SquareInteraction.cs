namespace Hattin.Extensions.SquareInteraction
{ //testing if you can overload ToString (aparently cant as you cant override while also being static)
    public static class SquareInteractionExtensions
    {
        public static string ToShortString(this Types.SquareInteraction interaction)
        {
            switch (interaction)
            {
                case Types.SquareInteraction.Attacking:
                    return "A";

                case Types.SquareInteraction.Defending:
                    return "D";

                case Types.SquareInteraction.ControllingEmpty:
                    return "C";

                case Types.SquareInteraction.OwnSquare:
                    return "O";
            }
            return "";
        }
    }
}