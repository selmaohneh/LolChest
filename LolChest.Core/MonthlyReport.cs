using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Storage.Net.Blobs;

namespace LolChest.Core
{
    public class MonthlyReport
    {
        private readonly IBlobStorage _blobStorage;

        public MonthlyReport(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        /// <param name="month">Format: yyyy-MM</param>
        public async Task<string> Create(string month)
        {
            List<SummonerResult> summonerResults = (await GetSummonerResultsFor(month)).ToList();

            summonerResults = summonerResults.GetLolChestSummonerResults().ToList();

            if (!summonerResults.Any())
            {
                return null;
            }

            string str = $"Hi!\n\nI am Cello's LolChest bot. Here are the final results for {month}:\n";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + " --> ";
                str += resultOfSingleSummoner.Sum(x => x.GetPenalty()) + "€";
                str += " (" + Math.Round(resultOfSingleSummoner.Average(x => x.GetPenalty()), 2) + "€ / game)";
            }

            str += "\n\nPlease transfer your penalty using the following link: https://www.paypal.com/paypalme/lolchest";
            str += $"\n\nJust in case you don't believe me, here is a list of all games from {month}:\n\n";

            str += summonerResults.Plot();

            return str;
        }

        private async Task<IEnumerable<SummonerResult>> GetSummonerResultsFor(string month)
        {
            DateTime dateTime = DateTime.Parse(month);
            string path = Path.Combine(dateTime.ToString("yyyy"), dateTime.ToString("MM"));

            var listOptions = new ListOptions
            {
                FilePrefix = path,
                Recurse = true,
            };

            var summonerResults = new List<SummonerResult>();

            var blobs = await _blobStorage.ListAsync(listOptions);

            foreach (Blob blob in blobs)
            {
                string json = await _blobStorage.ReadTextAsync(blob.FullPath);
                var summonerResult = JsonConvert.DeserializeObject<SummonerResult>(json);
                summonerResults.Add(summonerResult);
            }

            return summonerResults;
        }
    }
}