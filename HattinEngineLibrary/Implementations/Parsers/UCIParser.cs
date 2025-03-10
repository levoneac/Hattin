using System.Text.RegularExpressions;
using HattinEngineLibrary.UCI;

namespace HattinEngineLibrary.Implementations.Parsers
{
    public static partial class UCIParser
    {
        public static readonly Regex NoSpecial = MyRegex();
        public static UCICommand GetUCICommand(string input)
        {
            UCIParseIntermediate parseIntermediate = ParseInput(input);

            //Make a proper constructor
            UCICommand command = new UCICommand();

            if (Enum.TryParse(typeof(UCICommandFromGUI), parseIntermediate.Command, true, out object? result))
            {
                command.CommandFromGUI = (UCICommandFromGUI)result;
            }
            //Parse options and values
            command.Moves = parseIntermediate.Moves;
            command.FEN = parseIntermediate.FEN;
            return command;
        }

        //Need to find a way to make this more solid
        public static UCIParseIntermediate ParseInput(string input)
        {
            input = NoSpecial.Replace(input, string.Empty);
            List<string> words = input.Split(" ").Where(w => w != "").ToList();
            string command = words.First();

            int optionIndex = words.IndexOf("name");
            string option = "";
            if (optionIndex != -1)
            {
                option = words.Count > (optionIndex + 1) ? words[optionIndex + 1] : "";
            }

            int valueIndex = words.IndexOf("value");
            string value = "";
            if (valueIndex != -1)
            {
                value = words.Count > (valueIndex + 1) ? words[valueIndex + 1] : "";
            }


            int startPosIndex = words.IndexOf("position");
            {
                string[] moves = [];
                if (words[startPosIndex + 1] == "startpos")
                {
                    if (words.Count > (startPosIndex + 2) && words[startPosIndex + 2] == "moves")
                    {
                        moves = [.. words[(startPosIndex + 3)..]];
                    }
                    return new UCIParseIntermediate(command, moves, option, value);
                }
                if (words[startPosIndex + 1] == "fen")
                {
                    int movesIndex = words.IndexOf("moves");
                    if (movesIndex != -1)
                    {
                        moves = [.. words[(movesIndex + 1)..]];
                    }
                    else { movesIndex = words.Count; }

                    return new UCIParseIntermediate(command, string.Join(" ", words[(startPosIndex + 2)..movesIndex]), moves, option, value);

                    //can contain moves after FEN as well
                }
            }
            return new UCIParseIntermediate(command, option, value);
        }

        [GeneratedRegex(@"[^0-9a-zA-Z\._\- ]")]
        private static partial Regex MyRegex();
    }
}