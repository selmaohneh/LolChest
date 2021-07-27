using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Storage.Net.Blobs;

namespace LolChest.Core
{
    public class DailyReport
    {
        private readonly IBlobStorage _blobStorage;

        public DailyReport(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        /// <param name="date">Format: yyyy-MM-dd</param>
        public async Task<string> Create(string date)
        {
            List<SummonerResult> summonerResults = (await GetSummonerResultsFor(date)).ToList();

            summonerResults = summonerResults.GetLolChestSummonerResults().ToList();

            if (!summonerResults.Any())
            {
                return null;
            }

            string str = $"Hi!\n\nI am Cello's LolChest bot. Here are the final results of the session from {date}:\n";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var singleSummonerResults = summonerResults.Of(summoner).ToList();

                if (!singleSummonerResults.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + " --> ";
                str += singleSummonerResults.Sum(x => x.GetPenalty()) + "€";
                str += " (" + Math.Round(singleSummonerResults.Average(x => x.GetPenalty()), 2) + "€ / game)";
            }

            str += $"\n\nJust in case you don't believe me, here is a list of all games from {date}:\n \n";

            str += summonerResults.Plot();

            return str;
        }

        private async Task<IEnumerable<SummonerResult>> GetSummonerResultsFor(string date)
        {
            DateTime dateTime = DateTime.Parse(date);
            string path = Path.Combine(dateTime.ToString("yyyy"), dateTime.ToString("MM"), dateTime.ToString("dd"));

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