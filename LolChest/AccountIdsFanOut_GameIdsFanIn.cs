using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace LolChest
{
    public class AccountIdsFanOutGameIdsFanIn
    {
        private readonly LolChestConfig _lolChestConfig;

        public AccountIdsFanOutGameIdsFanIn(LolChestConfig lolChestConfig)
        {
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("AccountIdsFanOutGameIdsFanIn")]
        public async Task<string[]> Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var logger = context.CreateReplaySafeLogger(log);
            var accountIds = context.GetInput<string[]>();

            logger.LogInformation($"Fan out account ids. {ObjectDumper.Dump(accountIds)}");

            var gameIdTasks = new List<Task<string[]>>();
            foreach (var accountId in accountIds)
            {
                var activityInput = (_lolChestConfig.Region, accountId);
                gameIdTasks.Add(context.CallActivityAsync<string[]>("GetGameIds", activityInput));
            }

            var gameIdsOfSummoners = await Task.WhenAll(gameIdTasks);

            logger.LogInformation($"Retrieved game ids of all summoners. {ObjectDumper.Dump(gameIdsOfSummoners)}");

            var allGameIds = gameIdsOfSummoners.SelectMany(x => x).Distinct();

            var commonGameIds = new List<string>();
            foreach (var gameId in allGameIds)
            {
                var isCommonGame = true;
                foreach (var gameIdsOfSummoner in gameIdsOfSummoners)
                {
                    isCommonGame &= gameIdsOfSummoner.Contains(gameId);
                }

                if (isCommonGame)
                {
                    commonGameIds.Add(gameId);
                }
            }

            logger.LogInformation($"Filtered common game ids. {ObjectDumper.Dump(commonGameIds)}");

            return commonGameIds.ToArray();
        }
    }
}