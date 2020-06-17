using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RiotSharp.Endpoints.Interfaces;

namespace LolChest
{
    /// <summary>
    /// This functions is triggered by new accountIds inside the
    /// queue named 'accountids'. It fetches the match list containing
    /// the last 100 matches of the corresponding
    /// accountId from the Riot API and puts the gameIds into a queue called
    /// 'gameids'.
    /// </summary>
    public class LoadGameIds
    {
        private readonly IMatchEndpoint _mathEndpoint;
        private readonly LolChestConfig _lolChestConfig;

        public LoadGameIds(IMatchEndpoint mathEndpoint, LolChestConfig lolChestConfig)
        {
            _mathEndpoint = mathEndpoint;
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("LoadGameIds")]
        public async Task Run(
            [QueueTrigger("accountids", Connection = "AzureWebJobsStorage")]
            string accountId,
            [Queue("gameids"), StorageAccount("AzureWebJobsStorage")]
            ICollector<string> gameIds,
            ILogger log)
        {
            var matchList = await _mathEndpoint.GetMatchListAsync(_lolChestConfig.Region, accountId);

            foreach (var matchReference in matchList.Matches)
            {
                gameIds.Add(matchReference.GameId.ToString());
            }
        }
    }
}