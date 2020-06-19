using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace LolChest
{
    /// <summary>
    /// This DurableClient starts the whole functions chain of
    /// updating the LolChest. It is triggered by a timer every 15 minutes.
    /// </summary>
    public static class TriggerUpdateLolChest
    {
        [FunctionName("TriggerUpdateLolChest")]
        public static Task Run(
            [TimerTrigger("0 */15 * * * *")] TimerInfo timer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("Triggering update of LolChest.");
            return starter.StartNewAsync("UpdateLolChest");
        }
    }
}