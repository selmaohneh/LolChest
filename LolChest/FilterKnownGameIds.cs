using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace LolChest
{
    public class FilterKnownGameIds
    {
        private readonly CloudTables _cloudTables;

        public FilterKnownGameIds(CloudTables cloudTables)
        {
            _cloudTables = cloudTables;
        }

        [FunctionName("FilterKnownGameIds")]
        public async Task<string[]> Run([ActivityTrigger] string[] gameIds, ILogger log)
        {
            var registeredGamesTable = await _cloudTables.Get("registeredmatches");

            var unknownGameIds = new List<string>();
            foreach (var gameId in gameIds)
            {
                var retrieveOperation = TableOperation.Retrieve(gameId, gameId, new List<string> {"RowKey"});
                var tableResult = await registeredGamesTable.ExecuteAsync(retrieveOperation);

                if (tableResult.Result == null)
                {
                    log.LogInformation($"Found unknown game id: {gameIds}");
                    unknownGameIds.Add(gameId);
                }
            }

            return unknownGameIds.ToArray();
        }
    }
}