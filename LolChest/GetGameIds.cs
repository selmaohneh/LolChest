using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using RiotSharp.Endpoints.Interfaces;
using RiotSharp.Misc;

namespace LolChest
{
    public class GetGameIds
    {
        private readonly IMatchEndpoint _mathEndpoint;

        public GetGameIds(IMatchEndpoint mathEndpoint)
        {
            _mathEndpoint = mathEndpoint;
        }

        [FunctionName("GetGameIds")]
        public async Task<string[]> Run([ActivityTrigger] (Region region, string accountId) args, ILogger log)
        {
            log.LogInformation($"Retrieving game ids for account id {args.accountId} from region {args.region}");
            var matchList = await _mathEndpoint.GetMatchListAsync(args.region, args.accountId);
            var gameIds = matchList.Matches.Select(x => x.GameId.ToString()).ToArray();
            log.LogInformation($"Retrieved game ids. {ObjectDumper.Dump(gameIds)}");
            return gameIds;
        }
    }
}