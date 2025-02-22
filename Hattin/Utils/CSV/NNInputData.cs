using CsvHelper.Configuration;

namespace Hattin.Utils.CSV
{
    public class NNInputData
    {
        public string FEN { get; set; }
        public int EvalCentipawns { get; set; }
    }

    public class NNInputDataMap : ClassMap<NNInputData>
    {
        public NNInputDataMap()
        {
            Map(m => m.FEN).Name("FEN");
            Map(m => m.EvalCentipawns).Name("Evaluation");
        }
    }
}