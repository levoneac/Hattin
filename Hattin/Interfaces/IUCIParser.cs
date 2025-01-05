using Hattin.Types;

namespace Hattin.Interfaces
{
    public interface IUCIParser
    {
        public UCICommand GetUCICommand(string input);
        public static abstract UCIParseIntermediate ParseInput(string input);
    }
}