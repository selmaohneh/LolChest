using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using RiotSharp.Endpoints.Interfaces;
using RiotSharp.Misc;

namespace LolChest
{
    public class AddMatchToChest
    {
        private readonly IMatchEndpoint _matchEndpoint;
        private readonly CloudTables _cloudTables;

        public AddMatchToChest(IMatchEndpoint matchEndpoint, CloudTables cloudTables)
        {
            _matchEndpoint = matchEndpoint;
            _cloudTables = cloudTables;
        }

        [FunctionName("AddMatchToChest")]
        public async Task Run([ActivityTrigger] (Region region, string gameId) input, ILogger log)
        {
            var match = await _matchEndpoint.GetMatchAsync(input.region, long.Parse(input.gameId));
            log.LogInformation($"Retrieved match with id {match.GameId}");

            var matchEntity = new MatchEntity
            {
                PartitionKey = match.GameId.ToString(),
                RowKey = match.GameId.ToString(),
                GameCreation = match.GameCreation,
                MatchJson = JsonConvert.SerializeObject(match)
            };

            var registeredMatchesTable = await _cloudTables.Get("registeredmatches");
            var insertOperation = TableOperation.Insert(matchEntity);
            log.LogInformation($"Inserting match {match.GameId} in table.");
            await registeredMatchesTable.ExecuteAsync(insertOperation);
            log.LogInformation($"Inserting match {match.GameId} succeeded!");
        }
    }
}