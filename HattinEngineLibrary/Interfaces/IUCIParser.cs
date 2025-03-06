using HattinEngineLibrary.UCI;

namespace HattinEngineLibrary.Interfaces
{
    public interface IUCIParser
    {
        public UCICommand GetUCICommand(string input);
        public static abstract UCIParseIntermediate ParseInput(string input);
    }
}