using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using RiotSharp.Endpoints.Interfaces;
using RiotSharp.Misc;

namespace LolChest
{
    public class GetAccountId
    {
        private readonly ISummonerEndpoint _summonerEndpoint;

        public GetAccountId(ISummonerEndpoint summonerEndpoint)
        {
            _summonerEndpoint = summonerEndpoint;
        }

        [FunctionName("GetAccountId")]
        public async Task<string> Run([ActivityTrigger] (Region region, string summonerName) input, ILogger log)
        {
            log.LogInformation($"Retrieving summoner with name {input.summonerName} from region {input.region}");
            var summoner = await _summonerEndpoint.GetSummonerByNameAsync(input.region, input.summonerName);
            log.LogInformation($"Summoner retrieved! Account id is {summoner.AccountId}");
            return summoner.AccountId;
        }
    }
}