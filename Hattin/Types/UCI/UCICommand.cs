namespace Hattin.Types
{
    public class UCICommand
    {
        public UCICommandFromGUI CommandFromGUI { get; set; }
        public string Option { get; set; }
        public string Value { get; set; }
        public string[] Moves { get; set; }

        public UCICommand()
        {
            CommandFromGUI = UCICommandFromGUI.NoCommand;
            Option = "";
            Value = "";
            Moves = [];
        }
    }
}