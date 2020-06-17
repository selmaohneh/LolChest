using System;
using System.Collections.Generic;
using System.Linq;
using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest.Console
{
    public class LolChestEntry
    {
        private readonly List<string> _summonerNames;
        public string GameId { get; }
        public DateTime GameCreation { get; }
        public TimeSpan GameDuration { get; }
        public bool Won { get; }
        public IEnumerable<PlayerStats> LolChestStats { get; }

        public LolChestEntry(Match match, IEnumerable<string> summonerNames)
        {
            _summonerNames = summonerNames.ToList();
            GameId = match.GameId.ToString();
            GameCreation = match.GameCreation;
            GameDuration = match.GameDuration;

            var playerStats = new List<PlayerStats>();
            foreach (var summonerName in _summonerNames)
            {
                var summonerPartId =
                    match.ParticipantIdentities.Single(x => x.Player.SummonerName == summonerName);
                var summonerStats = match.Participants.Single(x => x.ParticipantId == summonerPartId.ParticipantId);

                Won = summonerStats.Stats.Winner;
                var playerStat = new PlayerStats(summonerName, summonerStats.Stats, GameDuration);
                playerStats.Add(playerStat);
            }

            LolChestStats = playerStats;
        }

        public override string ToString()
        {
            var plot = String.Empty;
            plot += $"GameId: {GameId}; ";
            plot += $"GameCreation: {GameCreation.ToLocalTime():dd.MM.yyyy HH:mm}; ";
            plot += $"GameDuration: {GameDuration.TotalMinutes:00.00}m;\n";

            foreach (var summonerName in _summonerNames)
            {
                var lolChestStat = LolChestStats.Single(x => x.SummonerName == summonerName);
                plot += $"{summonerName} ";
                plot += $"({lolChestStat.Kda.Kills} / {lolChestStat.Kda.Deaths} / {lolChestStat.Kda.Assists}): ";
                plot += $"{lolChestStat.Penalty:0.00}€\n";
            }

            return plot;
        }
    }
}