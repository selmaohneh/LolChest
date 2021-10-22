using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames.SummonerV4;

namespace LolChest.Core
{
    /// <summary>
    /// Polls the recent matches of all given summoners,
    /// converts them into a LolChest-compatible format (<see cref="SummonerResult"/>),
    /// and saves them to the given <see cref="ISummonerResultBucket"/>.
    /// </summary>
    public class Update
    {
        private readonly IRiotGamesApi _riotGamesApi;
        private readonly ISummonerResultBucket _summonerResultBucket;

        public Update(IRiotGamesApi riotGamesApi, ISummonerResultBucket summonerResultBucket)
        {
            _riotGamesApi = riotGamesApi;
            _summonerResultBucket = summonerResultBucket;
        }

        public async Task<string> Execute(PlatformRoute platformRoute, RegionalRoute regionalRoute, IEnumerable<string> summonerNames)
        {
            var summonerIds = (await GetSummonerIds(platformRoute, summonerNames)).ToList();
            var recentMatchIds = await GetRecentMatchIds(regionalRoute, summonerIds);
            var recentMatches = await GetRecentMatches(regionalRoute, recentMatchIds);
            var summonerResults = ConvertToSummonerResult(recentMatches, summonerIds).ToList();
            await SaveMatchResults(summonerResults);

            return summonerResults.Plot();
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
                    Participant matchInfoParticipant =
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
                await _summonerResultBucket.Save(summonerResult);
            }
        }
    }
}