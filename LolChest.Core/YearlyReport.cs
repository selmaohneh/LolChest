using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LolChest.Core
{
    public class YearlyReport
    {
        private readonly ISummonerResultBucket _summonerResultBucket;

        public YearlyReport(ISummonerResultBucket summonerResultBucket)
        {
            _summonerResultBucket = summonerResultBucket;
        }

        /// <param name="year">Format: yyyy</param>
        public async Task<string> Create(string year)
        {
            List<SummonerResult> summonerResults = (await _summonerResultBucket.GetForYear(year)).ToList();

            summonerResults = summonerResults.GetLolChestSummonerResults().ToList();

            if (!summonerResults.Any())
            {
                return null;
            }

            string str = $"Happy new year!\n\nI am Cello's LolChest bot. Here are some statistics for {year}:{Environment.NewLine}{Environment.NewLine}";

            str = AddGameCounts(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddWinRates(str, summonerResults);

            return str;
        }

        private string AddWinRates(string str, List<SummonerResult> summonerResults)
        {
            str += $"Total win rate: {Math.Round(summonerResults.GetWinRate() * 100, 2)}%";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + $" --> {Math.Round(resultOfSingleSummoner.GetWinRate() * 100, 2)}%";
            }

            return str;
        }

        private string AddGameCounts(string str, List<SummonerResult> summonerResults)
        {
            str += $"Total amount of games played: {summonerResults.CountGames()}";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + $" --> {resultOfSingleSummoner.CountGames()}";
            }

            return str;
        }
    }
}