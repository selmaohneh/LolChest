using System;
using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest.Console
{
    public class PlayerStats
    {
        private readonly TimeSpan _matchDuration;
        private readonly bool _won;

        public PlayerStats(string summonerName, ParticipantStats stats, TimeSpan matchDuration)
        {
            _matchDuration = matchDuration;
            _won = stats.Winner;
            SummonerName = summonerName;
            Kda = new Kda(stats);
        }

        public Kda Kda { get; }
        public string SummonerName { get; }
        public decimal Penalty => CalculatePenalty(Kda, _matchDuration, _won);

        private decimal CalculatePenalty(Kda kda, TimeSpan matchDuration,
            bool won)
        {
            var basePenality = kda.Deaths / (kda.Kills + kda.Assists + 1.0) *
                               matchDuration.TotalMinutes * 0.05;

            if (won)
            {
                return (decimal) basePenality;
            }

            return (decimal) (1.5 * basePenality);
        }
    }
}