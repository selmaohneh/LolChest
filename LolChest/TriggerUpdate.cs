using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LolChest
{
    /// <summary>
    /// This is the initial function that is triggered
    /// every 12 hours. It simply adds the configured summoner
    /// names into the queue with the name 'summonernames'. This
    /// will kickstart the whole function chain.
    /// </summary>
    public class TriggerUpdate
    {
        private readonly LolChestConfig _lolChestConfig;

        public TriggerUpdate(LolChestConfig lolChestConfig)
        {
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("TriggerUpdate")]
        public void Run([TimerTrigger("0 0 */12 * * *")] TimerInfo timer, ILogger log,
            [Queue("summonernames"), StorageAccount("AzureWebJobsStorage")]
            ICollector<string> summonerNames)
        {
            foreach (var summonerName in _lolChestConfig.SummonerNames)
            {
                summonerNames.Add(summonerName);
            }
        }
    }
}