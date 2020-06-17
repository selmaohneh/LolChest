using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using RiotSharp.Endpoints.Interfaces;

namespace LolChest
{
    /// <summary>
    /// This functions is triggered by new gameIds inside the queue named
    /// 'gameids'. It then checks whether this gameId was processed before by
    /// looking at the table 'processedgamdids'. If not it retrieves the match
    /// data from the Riot API. If all configured summoners played insided the
    /// retrieved match it will be added to the table 'registeredmachtes'.
    /// </summary>
    public class LoadMatches
    {
        private readonly IMatchEndpoint _matchEndpoint;
        private readonly CloudTables _tables;
        private readonly LolChestConfig _lolChestConfig;

        public LoadMatches(IMatchEndpoint matchEndpoint, CloudTables tables, LolChestConfig lolChestConfig)
        {
            _matchEndpoint = matchEndpoint;
            _tables = tables;
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("LoadMatches")]
        public async Task Run([QueueTrigger("gameids", Connection = "AzureWebJobsStorage")]
            string gameId, ILogger log)
        {
            var processedGameIds = await _tables.Get("processedgameids");
            var retrieveOperation = TableOperation.Retrieve(gameId, gameId);
            var existingMatch = await processedGameIds.ExecuteAsync(retrieveOperation);

            if (existingMatch.Result != null)
            {
                log.LogInformation($"GameId {gameId} was already loaded in the past.");
                return;
            }

            var match = await _matchEndpoint.GetMatchAsync(_lolChestConfig.Region, long.Parse(gameId));
            var insertOperation = TableOperation.Insert(new TableEntity(gameId, gameId));
            await processedGameIds.ExecuteAsync(insertOperation);

            foreach (var summonerName in _lolChestConfig.SummonerNames)
            {
                if (match.ParticipantIdentities.All(x => x.Player.SummonerName != summonerName))
                {
                    log.LogInformation($"{summonerName} did not play in match with GameId {gameId}");
                    return;
                }
            }

            var matchEntity = new MatchEntity
            {
                PartitionKey = match.GameId.ToString(),
                RowKey = match.GameId.ToString(),
                GameCreation = match.GameCreation,
                MatchJson = JsonConvert.SerializeObject(match)
            };

            var registeredMatches = await _tables.Get("registeredmatches");
            insertOperation = TableOperation.Insert(matchEntity);
            await registeredMatches.ExecuteAsync(insertOperation);
        }
    }
}