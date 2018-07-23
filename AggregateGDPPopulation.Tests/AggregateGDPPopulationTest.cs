using System;
using Xunit;
using AggregateGDPPopulation;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void ShouldBeAbleToCreateAggregateJsonFile()
        {
            Aggregate AggObj = new Aggregate();

            await AggObj.GenerateDatafile(@"../../../../AggregateGDPPopulation/data/datafile.csv", @"../../../../AggregateGDPPopulation/data/mapper.json");

            StreamReader file1 = new StreamReader(@"../../../../AggregateGDPPopulation/data/output.json");
            StreamReader file2 = new StreamReader(@"../../../../AggregateGDPPopulation/data/expected-output.json");

            string json1 = file1.ReadToEnd();
            string json2 = file2.ReadToEnd();

            file1.Close();
            file2.Close();

            var dic1 = JsonConvert.DeserializeObject<Dictionary<string, Data>>(json1);
            var dic2 = JsonConvert.DeserializeObject<Dictionary<string, Data>>(json2);

            bool dictionariesEqual = (dic1.Keys.Count == dic2.Keys.Count && dic1.Keys.All(k => dic2.ContainsKey(k) && dic1[k].IsEquals(dic2[k])));
            Assert.True(dictionariesEqual);
        }
    }
}
