using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RiotSharp.Endpoints.Interfaces;

namespace LolChest
{
    /// <summary>
    /// This functions is triggered by new summoner names inside the
    /// queue named 'summonernanes'. It fetches the corresponding
    /// accountId from the Riot API and puts it into a queue called
    /// 'accountids'.
    /// </summary>
    public class LoadAccountIds
    {
        private readonly ISummonerEndpoint _summonerEndpoint;
        private readonly LolChestConfig _config;

        public LoadAccountIds(ISummonerEndpoint summonerEndpoint, LolChestConfig config)
        {
            _summonerEndpoint = summonerEndpoint;
            _config = config;
        }

        [FunctionName("LoadAccountIds")]
        public async Task Run(
            [QueueTrigger("summonernames", Connection = "AzureWebJobsStorage")]
            string summonerName,
            [Queue("accountids"), StorageAccount("AzureWebJobsStorage")]
            ICollector<string> accountIds,
            ILogger log)
        {
            var summoner = await _summonerEndpoint.GetSummonerByNameAsync(_config.Region, summonerName);

            accountIds.Add(summoner.AccountId);
        }
    }
}