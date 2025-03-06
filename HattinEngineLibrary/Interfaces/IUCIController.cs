namespace HattinEngineLibrary.Interfaces
{
    public interface IUCIController
    {
        public void StartListening();
        public void SendOutput(string[] output);
    }
}