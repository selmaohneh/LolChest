using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames.SummonerV4;
using Newtonsoft.Json;
using Storage.Net.Blobs;

namespace LolChest.Core
{
    /// <summary>
    /// Polls the recent matches of all given summoners,
    /// converts them into a LolChest-compatible format (<see cref="SummonerResult"/>),
    /// and saves them to the given <see cref="IBlobStorage"/>.
    /// </summary>
    public class Update
    {
        private readonly IRiotGamesApi _riotGamesApi;
        private readonly IBlobStorage _blobStorage;

        public Update(IRiotGamesApi riotGamesApi, IBlobStorage blobStorage)
        {
            _riotGamesApi = riotGamesApi;
            _blobStorage = blobStorage;
        }

        public async Task Execute(PlatformRoute platformRoute, RegionalRoute regionalRoute, IEnumerable<string> summonerNames)
        {
            var summonerIds = (await GetSummonerIds(platformRoute, summonerNames)).ToList();
            var recentMatchIds = await GetRecentMatchIds(regionalRoute, summonerIds);
            var recentMatches = await GetRecentMatches(regionalRoute, recentMatchIds);
            var matchResults = ConvertToSummonerResult(recentMatches, summonerIds);
            await SaveMatchResults(matchResults);
        }

        private async Task<IEnumerable<string>> GetSummonerIds(PlatformRoute platformRoute, IEnumerable<string> summonerNames)
        {
            var ids = new List<string>();

            foreach (string summonerName in summonerNames)
            {
                Summoner summoner = await _riotGamesApi.SummonerV4().GetBySummonerNameAsync(platformRoute, summonerName);
                ids.Add(summoner.Puuid);
            }

            return ids;
        }

        private async Task<IEnumerable<string>> GetRecentMatchIds(RegionalRoute regionalRoute, IEnumerable<string> summonerIds)
        {
            var recentMatchesIds = new List<string>();

            foreach (string summonerId in summonerIds)
            {
                IEnumerable<string> matchIds = await _riotGamesApi.MatchV5().GetMatchIdsByPUUIDAsync(regionalRoute, summonerId);
                recentMatchesIds.AddRange(matchIds);
            }

            return recentMatchesIds;
        }

        private async Task<IEnumerable<Match>> GetRecentMatches(RegionalRoute regionalRoute, IEnumerable<string> recentMatchIds)
        {
            var matches = new List<Match>();

            foreach (string recentMatchId in recentMatchIds)
            {
                Match match = await _riotGamesApi.MatchV5().GetMatchAsync(regionalRoute, recentMatchId);
                matches.Add(match);
            }

            return matches;
        }

        private IEnumerable<SummonerResult> ConvertToSummonerResult(IEnumerable<Match> matches, IEnumerable<string> summonerIds)
        {
            var summonerResults = new List<SummonerResult>();

            foreach (Match match in matches)
            {
                DateTime creation = DateTimeOffset.FromUnixTimeMilliseconds(match.Info.GameCreation).DateTime.AddHours(2);
                TimeSpan duration = TimeSpan.FromMilliseconds(match.Info.GameDuration);

                foreach (string summonerId in summonerIds)
                {
                    MatchInfoParticipant matchInfoParticipant =
                        match.Info.Participants.SingleOrDefault(x => x.Puuid == summonerId);

                    if (matchInfoParticipant == null)
                    {
                        continue;
                    }

                    bool win = matchInfoParticipant.Win;
                    var kda = new Kda(matchInfoParticipant.Kills, matchInfoParticipant.Deaths, matchInfoParticipant.Assists);

                    var summonerResult = new SummonerResult(matchInfoParticipant.SummonerName,
                                                            (int)matchInfoParticipant.ChampionId,
                                                            kda,
                                                            duration,
                                                            win,
                                                            match.Info.GameId.ToString(),
                                                            creation);
                    summonerResults.Add(summonerResult);
                }
            }

            return summonerResults;
        }

        private async Task SaveMatchResults(IEnumerable<SummonerResult> summonerResults)
        {
            foreach (SummonerResult summonerResult in summonerResults)
            {
                string path = Path.Combine(summonerResult.GameCreation.ToString("yyyy"),
                                           summonerResult.GameCreation.ToString("MM"),
                                           summonerResult.GameCreation.ToString("dd"),
                                           summonerResult.SummonerName);
                string file = Path.Combine(path, $"{summonerResult.GameId}.lol");

                string json = JsonConvert.SerializeObject(summonerResult, Formatting.Indented);

                await _blobStorage.WriteTextAsync(file,
                                                  json);
            }
        }
    }
}