using System;
using System.Linq;
using System.Threading.Tasks;

namespace LolChest.Core
{
    public class DailyReport
    {
        private readonly ISummonerResultBucket _summonerResultBucket;

        public DailyReport(ISummonerResultBucket summonerResultBucket)
        {
            _summonerResultBucket = summonerResultBucket;
        }

        /// <param name="date">Format: yyyy-MM-dd</param>
        public async Task<string> Create(string date)
        {
            var summonerResults = (await _summonerResultBucket.GetForDay(date)).ToList();

            var validSummonerResults = summonerResults.GetLolChestSummonerResults().ToList();

            if (!validSummonerResults.Any())
            {
                return null;
            }

            string str = $"Hi!\n\nI am Cello's LolChest bot. Here are the final results of the session from {date}:\n";

            foreach (string summoner in validSummonerResults.GetSummoners())
            {
                var singleSummonerResults = validSummonerResults.Of(summoner).ToList();

                if (!singleSummonerResults.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + " --> ";
                str += singleSummonerResults.Sum(x => x.GetPenalty()) + "€";
                str += " (" + Math.Round(singleSummonerResults.Average(x => x.GetPenalty()), 2) + "€ / game)";
            }

            str += $"\n\nJust in case you don't believe me, here is a list of all games from {date}:\n \n";

            str += validSummonerResults.Plot();

            return str;
        }
    }
}