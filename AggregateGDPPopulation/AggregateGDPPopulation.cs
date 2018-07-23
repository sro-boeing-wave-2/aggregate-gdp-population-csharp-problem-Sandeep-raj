using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace AggregateGDPPopulation
{
    public class Aggregate
    {
        public async Task GenerateDatafile(string DatafilePath, string MapperfilePath)
        {
            StreamReader DatafileReader = new StreamReader(DatafilePath);
            StreamReader MapperfileReader = new StreamReader(MapperfilePath);
            Task<string> DatafileTask = DatafileReader.ReadToEndAsync();
            Task<string> MapperfileTask = MapperfileReader.ReadToEndAsync();
            Task data = Task.WhenAll(DatafileTask, MapperfileTask);
            data.Wait();
            string DataString = DatafileTask.Result;
            string MapperString = MapperfileTask.Result;
            DatafileReader.Close();
            MapperfileReader.Close();

            List<string[]> Records = new List<string[]>();

            string[] dataRecord = DataString.Replace("\"", string.Empty).Split('\n');

            foreach (string record in dataRecord)
            {
                Records.Add(record.Split(','));
            }

            JObject jsonMapper = JObject.Parse(MapperString);

            Dictionary<string, Data> AggregateDict = new Dictionary<string, Data>();
            int countryindex = Array.IndexOf(Records[0], "Country Name");
            int populationindex = Array.IndexOf(Records[0], "Population (Millions) 2012");
            int gdpindex = Array.IndexOf(Records[0], "GDP Billions (USD) 2012");

            for (int i = 1; i < Records.Count; i++)
            {
                if (jsonMapper[Records[i][countryindex]] != null)
                {
                    if (AggregateDict.ContainsKey(jsonMapper[Records[i][countryindex]].ToString()))
                    {
                        AggregateDict[jsonMapper[Records[i][countryindex]].ToString()].GDP_2012 += Math.Round(Convert.ToDouble(Records[i][gdpindex]));
                        AggregateDict[jsonMapper[Records[i][countryindex]].ToString()].POPULATION_2012 += Math.Round(Convert.ToDouble(Records[i][populationindex]));
                    }
                    else
                    {
                        AggregateDict[jsonMapper[Records[i][countryindex]].ToString()] = new Data();
                        AggregateDict[jsonMapper[Records[i][countryindex]].ToString()].GDP_2012 = Math.Round(Convert.ToDouble(Records[i][gdpindex]));
                        AggregateDict[jsonMapper[Records[i][countryindex]].ToString()].POPULATION_2012 = Math.Round(Convert.ToDouble(Records[i][populationindex]));
                    }
                }
            }

            string json = JsonConvert.SerializeObject(AggregateDict, Formatting.Indented);
            StreamWriter OutputStream = new StreamWriter(@"../../../../AggregateGDPPopulation/data/output.json");
            await OutputStream.WriteAsync(json);
            OutputStream.Close();
        }
    }

    public class Data
    {
        public double GDP_2012 { get; set; }
        public double POPULATION_2012 { get; set; }

        public bool IsEquals(object obj)
        {
            Data d1 = (Data)obj;
            if (this.GDP_2012 == d1.GDP_2012 && this.POPULATION_2012 == d1.POPULATION_2012)
                return true;
            return false;
        }
    }
}