namespace Hattin.Types
{
    public class UCIParseIntermediate
    {
        public string Command { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }

        public UCIParseIntermediate(string command, string optionName, string optionValue)
        {
            Command = command;
            OptionName = optionName;
            OptionValue = optionValue;
        }
    }
}