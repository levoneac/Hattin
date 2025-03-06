using CsvHelper.Configuration;

namespace HattinEngineLibrary.Utils.CSV
{
    public class NNOutputData
    {
        public int[] NNInput { get; set; }
        public int EvalCentipawns { get; set; }
    }

    public class NNOutputDataMap : ClassMap<NNOutputData>
    {
        public NNOutputDataMap()
        {
            Map(m => m.NNInput).Index(0).Name("NNInput");
            Map(m => m.EvalCentipawns).Index(1).Name("EvalCentipawns");
        }
    }

}