using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.Parsers
{
    public static class UCIParser
    {

        public static UCICommand GetUCICommand(string input)
        {
            UCIParseIntermediate parseIntermediate = ParseInput(input);

            if (Enum.TryParse(typeof(UCICommandFromGUI), parseIntermediate.Command, true, out object? result))
            {
                return new UCICommand((UCICommandFromGUI)result);
            }
            return new UCICommand(UCICommandFromGUI.NoCommand);
        }

        //Bare minimum for now
        public static UCIParseIntermediate ParseInput(string input)
        {
            List<string> words = input.ToUpper().Split(" ").Where(w => w != "").ToList();
            string command = words.First();

            int optionIndex = words.IndexOf("NAME");
            string option = "";
            if (optionIndex != -1)
            {
                option = words.Count > (optionIndex + 1) ? words[optionIndex + 1] : "";

            }

            int valueIndex = words.IndexOf("VALUE");
            string value = "";
            if (valueIndex != -1)
            {
                value = words.Count > (valueIndex + 1) ? words[valueIndex + 1] : "";
            }

            return new UCIParseIntermediate(command, option, value);
        }
    }
}