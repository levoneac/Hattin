namespace Hattin.Types
{
    public class UCICommand
    {
        public UCICommandFromGUI CommandFromGUI { get; set; }
        public string? OptionPlaceholder;
        public string? ValuePlaceHolder;

        public UCICommand(UCICommandFromGUI commandFromGUI, string? placeholder = null, string? placeholder2 = null)
        {
            CommandFromGUI = commandFromGUI;
        }
    }
}