using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Hattin.Types;

namespace Hattin.Utils.CSV
{
    public class NNCsvHandler
    {
        private BoardState Board;

        public NNCsvHandler(BoardState board)
        {
            Board = board;
        }

        public void ReadConvertAndWrite(string pathToInput, string pathToOutput, long maxRows)
        {
            List<NNOutputData> outputData = new List<NNOutputData>();

            //Read and convert
            using (StreamReader streamReader = new StreamReader(pathToInput))
            {
                using (CsvReader reader = LoadFile(streamReader))
                {
                    for (int i = 0; reader.Read() && i < maxRows; i++)
                    {
                        NNInputData data = reader.GetRecord<NNInputData>();
                        outputData.Add(new NNOutputData()
                        {
                            NNInput = ConvertFENToNNInput(data.FEN),
                            EvalCentipawns = data.EvalCentipawns
                        });
                    }
                }
            }

            //Write
            using (StreamWriter streamWriter = new StreamWriter(pathToOutput))
            {
                WriteFile(streamWriter, outputData);
            }

        }

        private CsvReader LoadFile(StreamReader streamReader)
        {
            CsvReader reader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            reader.Context.RegisterClassMap<NNInputDataMap>();
            return reader;
        }

        private void WriteFile(StreamWriter streamWriter, List<NNOutputData> data)
        {
            using (CsvWriter writer = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false
            }))
            {
                writer.Context.RegisterClassMap<NNOutputDataMap>();
                writer.WriteRecords(data);
            }
        }

        private int[] ConvertFENToNNInput(string FEN)
        {
            Board.ProcessFEN(FEN);
            int[] ret = new int[772];
            Array.Copy(Board.NeuralNetRepresentation.NeuralInput, ret, 772);
            return ret;
        }

    }
}