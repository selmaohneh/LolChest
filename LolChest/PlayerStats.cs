using System;
using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest
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
                return RoundTo2Digits(basePenality);
            }

            return RoundTo2Digits(1.5 * basePenality);
        }

        private decimal RoundTo2Digits(double value)
        {
            return Math.Round((decimal)value, 2);
        }
    }
}