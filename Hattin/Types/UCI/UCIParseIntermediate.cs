namespace Hattin.Types
{
    public class UCIParseIntermediate
    {
        public string Command { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }
        public string[] Moves { get; set; }
        public string? FEN { get; set; }

        public UCIParseIntermediate(string command, string optionName = "", string optionValue = "")
        {
            Command = command;
            OptionName = optionName;
            OptionValue = optionValue;
            Moves = [];
            FEN = null;
        }

        public UCIParseIntermediate(string command, string[] moves, string optionName = "", string optionValue = "")
        {
            Command = command;
            OptionName = optionName;
            OptionValue = optionValue;
            Moves = moves;
            FEN = null;
        }
        public UCIParseIntermediate(string command, string? fen, string optionName = "", string optionValue = "")
        {
            Command = command;
            OptionName = optionName;
            OptionValue = optionValue;
            Moves = [];
            FEN = fen;
        }
    }
}