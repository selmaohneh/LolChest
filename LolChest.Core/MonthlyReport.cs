using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LolChest.Core
{
    public class MonthlyReport
    {
        private readonly ISummonerResultBucket _summonerResultBucket;

        public MonthlyReport(ISummonerResultBucket summonerResultBucket)
        {
            _summonerResultBucket = summonerResultBucket;
        }

        /// <param name="month">Format: yyyy-MM</param>
        public async Task<string> Create(string month)
        {
            List<SummonerResult> summonerResults = (await _summonerResultBucket.GetForMonth(month)).ToList();

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
    }
}