using System.Buffers.Text;

namespace Hattin.Interfaces
{
    public interface IUCIParser
    {
        public void StartListening();
        public string? ParseInput(string input);
        public void SendOutput(string output);
    }
}