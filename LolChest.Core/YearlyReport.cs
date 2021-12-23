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
            str = AddLongestGame(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddFastestWin(str, summonerResults);
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
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddKillRecords(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddDeathRecords(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddAssistsRecords(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddHighestKdaRecords(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddPenaltyRecords(str, summonerResults);
            str += Environment.NewLine;
            str += Environment.NewLine;
            str = AddMostPlayedChampion(str, summonerResults);

            return str;
        }

        private string AddMostPlayedChampion(string str, List<SummonerResult> summonerResults)
        {
            var playedChampions = summonerResults.Select(x => x.ChampionId).GroupBy(x => x).OrderByDescending(x => x.Count()).ToList();
            str += $"Most played champion: {playedChampions.First().Key.GetFriendlyName()} ({playedChampions.First().Count()} times)";

            return str;
        }

        private string AddHighestKdaRecords(string str, List<SummonerResult> summonerResults)
        {
            double most = summonerResults.Max(x => x.Kda.GetSingleDigitKda());
            var results = summonerResults.Where(x => x.Kda.GetSingleDigitKda() == most);

            str += $"Highest K/D/A record: {Math.Round(most, 2)}";

            foreach (SummonerResult result in results)
            {
                str += Environment.NewLine;
                str += result.ToStringWithoutPenalty();
            }

            return str;
        }

        private string AddPenaltyRecords(string str, List<SummonerResult> summonerResults)
        {
            decimal most = summonerResults.Max(x => x.GetPenalty());
            var results = summonerResults.Where(x => x.GetPenalty() == most);

            str += $"Highest penalty record: {Math.Round(most, 2)}€";

            foreach (SummonerResult result in results)
            {
                str += Environment.NewLine;
                str += result.ToStringWithoutPenalty();
            }

            return str;
        }

        private string AddFastestWin(string str, List<SummonerResult> summonerResults)
        {
            TimeSpan duration = summonerResults.Where(x => x.Won).Min(x => x.GameDuration);

            IEnumerable<SummonerResult> resultWithLongestDuration =
                summonerResults.Where(x => x.Won).Where(x => x.GameDuration == duration).GroupBy(x => x.GameId).First();
            str += $"Fastest win: {Math.Round(duration.TotalMinutes, 0)} minutes";

            foreach (SummonerResult result in resultWithLongestDuration)
            {
                str += Environment.NewLine;
                str += result.ToStringWithoutPenalty();
            }

            return str;
        }

        private string AddLongestGame(string str, List<SummonerResult> summonerResults)
        {
            TimeSpan duration = summonerResults.Max(x => x.GameDuration);
            IEnumerable<SummonerResult> resultWithLongestDuration = summonerResults.Where(x => x.GameDuration == duration).GroupBy(x => x.GameId).First();
            str += $"Longest game: {Math.Round(duration.TotalMinutes, 0)} minutes";

            foreach (SummonerResult result in resultWithLongestDuration)
            {
                str += Environment.NewLine;
                str += result.ToStringWithoutPenalty();
            }

            return str;
        }

        private string AddKillRecords(string str, List<SummonerResult> summonerResults)
        {
            int mostKills = summonerResults.Max(x => x.Kda.Kills);
            var resultsWithMostKills = summonerResults.Where(x => x.Kda.Kills == mostKills);

            str += $"Most kills record: {mostKills}";

            foreach (SummonerResult resultsWithMostKill in resultsWithMostKills)
            {
                str += Environment.NewLine;
                str += resultsWithMostKill.ToStringWithoutPenalty();
            }

            return str;
        }

        private string AddDeathRecords(string str, List<SummonerResult> summonerResults)
        {
            int mostDeaths = summonerResults.Max(x => x.Kda.Deaths);
            var resultsWithMostDeaths = summonerResults.Where(x => x.Kda.Deaths == mostDeaths);

            str += $"Most deaths record: {mostDeaths}";

            foreach (SummonerResult resultsWithMostDeath in resultsWithMostDeaths)
            {
                str += Environment.NewLine;
                str += resultsWithMostDeath.ToStringWithoutPenalty();
            }

            return str;
        }

        private string AddAssistsRecords(string str, List<SummonerResult> summonerResults)
        {
            int mostAssists = summonerResults.Max(x => x.Kda.Assists);
            var resultsWithMostAssists = summonerResults.Where(x => x.Kda.Assists == mostAssists);

            str += $"Most assists record: {mostAssists}";

            foreach (SummonerResult resultsWithMostAssist in resultsWithMostAssists)
            {
                str += Environment.NewLine;
                str += resultsWithMostAssist.ToStringWithoutPenalty();
            }

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