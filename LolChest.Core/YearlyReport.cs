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

            str = AddTotalPenalty(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddGameCounts(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddTotalDuration(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddWinRates(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddTotalAmountOfKills(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddTotalAmountOfDeaths(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddTotalAmountOfAssists(str, summonerResults);

            return str;
        }

        private string AddTotalDuration(string str, List<SummonerResult> summonerResults)
        {
            TimeSpan duration = summonerResults.GetTotalDuration();
            str += $"Total time played: {duration.Days} days, {duration.Hours} hours, and {duration.Minutes} minutes.";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                TimeSpan summonerDuration = resultOfSingleSummoner.GetTotalDuration();

                str += Environment.NewLine
                     + summoner
                     + $" --> {summonerDuration.Days} days, {summonerDuration.Hours} hours, and {summonerDuration.Minutes} minutes.";
            }

            return str;
        }

        private string AddTotalAmountOfKills(string str, List<SummonerResult> summonerResults)
        {
            str += $"Total amount of kills: {summonerResults.Sum(x => x.Kda.Kills)}";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + $" --> {resultOfSingleSummoner.Sum(x => x.Kda.Kills)}";
            }

            return str;
        }

        private string AddTotalAmountOfDeaths(string str, List<SummonerResult> summonerResults)
        {
            str += $"Total amount of deaths: {summonerResults.Sum(x => x.Kda.Deaths)}";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + $" --> {resultOfSingleSummoner.Sum(x => x.Kda.Deaths)}";
            }

            return str;
        }

        private string AddTotalAmountOfAssists(string str, List<SummonerResult> summonerResults)
        {
            str += $"Total amount of assists: {summonerResults.Sum(x => x.Kda.Assists)}";

            foreach (string summoner in summonerResults.GetSummoners())
            {
                var resultOfSingleSummoner = summonerResults.Of(summoner).ToList();

                if (!resultOfSingleSummoner.Any())
                {
                    continue;
                }

                str += Environment.NewLine + summoner + $" --> {resultOfSingleSummoner.Sum(x => x.Kda.Assists)}";
            }

            return str;
        }

        private string AddTotalPenalty(string str, List<SummonerResult> summonerResults)
        {
            str += $"Total income: {Math.Round(summonerResults.Sum(x => x.GetPenalty()), 2)}€";
            str += " (" + Math.Round(summonerResults.Sum(x => x.GetPenalty()) / summonerResults.CountGames(), 2) + "€ / game)";

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