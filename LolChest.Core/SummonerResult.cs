using System;
using System.Collections.Generic;
using System.Linq;

namespace LolChest.Core
{
    /// <summary>
    /// Represents the results a single match of a single summoner.
    /// </summary>
    public class SummonerResult
    {
        public SummonerResult(string summonerName,
                              int championId,
                              Kda kda,
                              TimeSpan gameDuration,
                              bool won,
                              string gameId,
                              DateTime gameCreation)
        {
            SummonerName = summonerName;

            if (SummonerName == "Holger Hodensack")
            {
                SummonerName = "DFF Fiesoduck";
            }

            if (SummonerName == "Whizzmaster")
            {
                SummonerName = "DFF Quackerjack";
            }

            ChampionId = championId;
            Kda = kda;
            GameDuration = gameDuration;
            Won = won;
            GameId = gameId;
            GameCreation = gameCreation;
        }

        public string GameId { get; }
        public DateTime GameCreation { get; }
        public string SummonerName { get; }
        public int ChampionId { get; }
        public Kda Kda { get; }
        public TimeSpan GameDuration { get; }
        public bool Won { get; }

        public override string ToString()
        {
            return $"{SummonerName} ({ChampionId.GetFriendlyName()}): {Kda} ==> {this.GetPenalty()}€";
        }

        public decimal GetPenalty()
        {
            double basePenality = Kda.Deaths
                                / (Kda.Kills + Kda.Assists + 1.0)
                                * GameDuration.TotalMinutes
                                * 0.05;

            if (Won)
            {
                return RoundTo2Digits(basePenality);
            }

            return RoundTo2Digits(1.5 * basePenality);
        }

        private static decimal RoundTo2Digits(double value)
        {
            return Math.Round((decimal)value, 2);
        }
    }

    public static class SummonerResultExtensions
    {
        /// <summary>
        /// Returns all summoners that are present inside the given list of <see cref="SummonerResult"/>
        /// </summary>
        public static IEnumerable<string> GetSummoners(this IEnumerable<SummonerResult> summonerResults)
        {
            return summonerResults.Select(x => x.SummonerName).Distinct();
        }

        /// <summary>
        /// Returns all <see cref="SummonerResult"/> of the given summoner.
        /// </summary>
        public static IEnumerable<SummonerResult> Of(this IEnumerable<SummonerResult> summonerResults, string summonerName)
        {
            return summonerResults.Where(x => x.SummonerName == summonerName);
        }

        /// <summary>
        /// Filters the given <see cref="SummonerResult"/> and returns only those that were played by at least two players.
        /// </summary>
        public static IEnumerable<SummonerResult> GetLolChestSummonerResults(this IEnumerable<SummonerResult> summonerResults)
        {
            var lolChestSummonerResults = summonerResults.GroupBy(x => x.GameId).Where(x => x.Count() >= 2).SelectMany(x => x);

            return lolChestSummonerResults;
        }

        public static string Plot(this IEnumerable<SummonerResult> summonerResults)
        {
            string str = String.Empty;

            var groups = summonerResults.GroupBy(x => x.GameId);

            foreach (IGrouping<string, SummonerResult> grouping in groups)
            {
                SummonerResult result = grouping.First();
                str += $"GameId: {result.GameId}" + Environment.NewLine;
                str += result.GameCreation.ToString("dd.MM.yyyy HH:mm") + Environment.NewLine;
                str += $"{(int)result.GameDuration.TotalMinutes} minutes" + Environment.NewLine;

                if (result.Won)
                {
                    str += "Result: Victory" + Environment.NewLine;
                }
                else
                {
                    str += "Result: Defeat" + Environment.NewLine;
                }

                foreach (SummonerResult summonerResult in grouping.Select(x => x))
                {
                    str += summonerResult + Environment.NewLine;
                }

                str += Environment.NewLine;
            }

            return str;
        }
    }
}