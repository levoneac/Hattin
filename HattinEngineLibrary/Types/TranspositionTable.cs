namespace HattinEngineLibrary.Types
{

    public class TranspositionTable<T> : Dictionary<int, T>
    {
        //public int CapacityBytes { get; }
        //private int EntrySizeBytes;
        public int CapacityEntries { get; }
        private Queue<int> KeyQueue;
        public new T? this[int key]
        {
            get
            {
                return base[key];
            }
            set
            {
                while (Count > CapacityEntries)
                {
                    int oldestKey = KeyQueue.Dequeue();
                    Remove(oldestKey);
                }
                base[key] = value;
                KeyQueue.Enqueue(key);
            }
        }
        public TranspositionTable(int maxEntries)
        {
            //Not sure if its possible to find the maxsize of a reference type
            //CapacityBytes = (1024 ^ 2) * maxSizeMB;
            //EntrySizeBytes = Marshal.SizeOf<T>();
            //CapacityEntries = CapacityBytes / EntrySizeBytes;

            CapacityEntries = maxEntries;
            KeyQueue = new Queue<int>();
        }
    }
}