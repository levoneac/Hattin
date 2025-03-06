namespace HattinEngineLibrary.UCI
{
    public class UCICommand
    {
        public UCICommandFromGUI CommandFromGUI { get; set; }
        public string Option { get; set; } //make enum
        public string Value { get; set; }
        public string[] Moves { get; set; }
        public string? FEN { get; set; }

        public UCICommand()
        {
            CommandFromGUI = UCICommandFromGUI.NoCommand;
            Option = "";
            Value = "";
            Moves = [];
            FEN = null;
        }
    }
}