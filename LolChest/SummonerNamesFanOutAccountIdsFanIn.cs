using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace LolChest
{
    public class SummonerNamesFanOutAccountIdsFanIn
    {
        private readonly LolChestConfig _lolChestConfig;

        public SummonerNamesFanOutAccountIdsFanIn(LolChestConfig lolChestConfig)
        {
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("SummonerNamesFanOutAccountIdsFanIn")]
        public async Task<string[]> Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var logger = context.CreateReplaySafeLogger(log);
            logger.LogInformation("Fan out summoner names to retrieve account ids.");

            var accountIdTasks = new List<Task<string>>();
            foreach (var summonerName in _lolChestConfig.SummonerNames)
            {
                var activityInput = (_lolChestConfig.Region, summonerName);
                accountIdTasks.Add(context.CallActivityAsync<string>("GetAccountId", activityInput));
            }

            var accountIds = await Task.WhenAll(accountIdTasks);
            logger.LogInformation($"Retrieved account ids. {ObjectDumper.Dump(accountIds)}");
            return accountIds;
        }
    }
}