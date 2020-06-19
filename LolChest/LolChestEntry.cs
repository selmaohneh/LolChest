using System;
using System.Collections.Generic;
using System.Linq;
using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest
{
    public class LolChestEntry
    {
        public string GameId { get; }
        public DateTime GameCreation { get; }
        public TimeSpan GameDuration { get; }
        public bool Won { get; }
        public IEnumerable<PlayerStats> LolChestStats { get; }

        public LolChestEntry(Match match, IEnumerable<string> summonerNames)
        {
            GameId = match.GameId.ToString();
            GameCreation = match.GameCreation.ToLocalTime();
            GameDuration = match.GameDuration;

            var playerStats = new List<PlayerStats>();
            foreach (var summonerName in summonerNames)
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
    }
}