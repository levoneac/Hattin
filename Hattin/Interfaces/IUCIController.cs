using System.Buffers.Text;

namespace Hattin.Interfaces
{
    public interface IUCIController
    {
        public void StartListening();
        public void SendOutput(string[] output);
    }
}