using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace LolChest
{
    public class GameIdsFanOutAddMatchToChestFanIn
    {
        private readonly LolChestConfig _lolChestConfig;

        public GameIdsFanOutAddMatchToChestFanIn(LolChestConfig lolChestConfig)
        {
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("GameIdsFanOutAddMatchToChestFanIn")]
        public async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var logger = context.CreateReplaySafeLogger(log);
            var gameIds = context.GetInput<string[]>();

            logger.LogInformation($"Fan out game ids to store them in chest. {ObjectDumper.Dump(gameIds)}");

            var addMatchToChestTasks = new List<Task>();
            foreach (var gameId in gameIds)
            {
                var activityInput = (_lolChestConfig.Region, gameId);
                addMatchToChestTasks.Add(context.CallActivityAsync("AddMatchToChest", activityInput));
            }

            await Task.WhenAll(addMatchToChestTasks);
            logger.LogInformation($"Game ids were added to the chest. {ObjectDumper.Dump(gameIds)}");
        }
    }
}