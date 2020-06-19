using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace LolChest
{
    public static class UpdateLolChest
    {
        [FunctionName("UpdateLolChest")]
        public static async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)

        {
            var logger = context.CreateReplaySafeLogger(log);

            var accountIds =
                await context.CallSubOrchestratorAsync<string[]>("SummonerNamesFanOutAccountIdsFanIn",
                    null);

            logger.LogInformation($"Retrieved account ids: {ObjectDumper.Dump(accountIds)}");

            var gameIds =
                await context.CallSubOrchestratorAsync<string[]>("AccountIdsFanOutGameIdsFanIn", accountIds);
            logger.LogInformation($"Retrieved game ids: {ObjectDumper.Dump(gameIds)}");

            var newGameIds =
                await context.CallActivityAsync<string[]>("FilterKnownGameIds", gameIds);
            logger.LogInformation($"Retrieved {newGameIds.Length} new game ids: {ObjectDumper.Dump(newGameIds)}");

            await context.CallSubOrchestratorAsync("GameIdsFanOutAddMatchToChestFanIn", newGameIds);
            logger.LogInformation("LolChest update succeeded!");
        }
    }
}