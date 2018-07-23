using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AggregateGDPPopulation
{
    public class Aggregate
    {
        public async Task<List<string[]>> ReadDataFile(string fp)
        {
            string line;
            List<string[]> l = new List<string[]>();
            StreamReader file = new StreamReader(fp);
            while ((line = file.ReadLine()) != null)
            {
                line = line.Replace("\"", string.Empty);
                string[] data = line.Split(',');
                l.Add(data);
            }
            file.Close();
            return l;
        }

        public async Task<Dictionary<string, string>> ReadMapperFile(string fp)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            string line, x = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            System.IO.StreamReader file = new System.IO.StreamReader(fp);
            while ((line = file.ReadLine()) != null)
            {
                line = line.Replace("\"", string.Empty);
                line = line.Replace("{", string.Empty);
                x = JsonConvert.SerializeObject(line);
                string[] d = x.Split(',');
                foreach (string s in d)
                {
                    string[] t = s.Split(':');
                    mapper[t[0]] = t[1];
                }
            }
            file.Close();
            return mapper;
        }

        public async Task GenerateDatafile()
        {

            Task<List<string[]>> ret1 = ReadDataFile(@"../../../../AggregateGDPPopulation/data/datafile.csv");
            Task<Dictionary<string, string>> ret2 = ReadMapperFile(@"../../../../AggregateGDPPopulation/data/mapper.json");

            List<string[]> l = await ret1;
            Dictionary<string, string> mapper = await ret2;


            Dictionary<string, data> dict = new Dictionary<string, data>();
            int countryindex = Array.IndexOf(l[0], "Country Name");
            int populationindex = Array.IndexOf(l[0], "Population (Millions) 2012");
            int gdpindex = Array.IndexOf(l[0], "GDP Billions (USD) 2012");

            for (int i = 1; i < l.Count; i++)
            {
                if (mapper.ContainsKey(l[i][countryindex]))
                {
                    if (dict.ContainsKey(mapper[l[i][countryindex]]))
                    {
                        dict[mapper[l[i][countryindex]]].GDP_2012 += Math.Round(Convert.ToDouble(l[i][gdpindex]));
                        dict[mapper[l[i][countryindex]]].POPULATION_2012 += Math.Round(Convert.ToDouble(l[i][populationindex]));
                    }
                    else
                    {
                        dict[mapper[l[i][countryindex]]] = new data();
                        dict[mapper[l[i][countryindex]]].GDP_2012 = Math.Round(Convert.ToDouble(l[i][gdpindex]));
                        dict[mapper[l[i][countryindex]]].POPULATION_2012 = Math.Round(Convert.ToDouble(l[i][populationindex]));
                    }
                }
            }

            await WriteJsonFile(dict);
        }

        public async Task WriteJsonFile(Dictionary<string,data> dict)
        {
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            StreamWriter file = new StreamWriter(@"../../../../AggregateGDPPopulation/data/output.json");
            file.Write(json);
            file.Close();
        }
    }

    public class data
    {
        public double GDP_2012 { get; set; }
        public double POPULATION_2012 { get; set; }

        public override bool Equals(object obj)
        {
            data d1 = (data)obj;
            if (this.GDP_2012 == d1.GDP_2012 && this.POPULATION_2012 == d1.POPULATION_2012)
                return true;
            return false;
        }
    }
}