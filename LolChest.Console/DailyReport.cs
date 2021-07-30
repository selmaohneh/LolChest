using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LolChest.Core;

namespace LolChest.Console
{
    [Command("daily", Description = "Shows the results of a given day.")]
    public class DailyReport : ICommand
    {
        [CommandParameter(1, Description = "The access key for Aws.")]
        public string AwsAccessKey { get; set; }

        [CommandParameter(2, Description = "The secret key for Aws.")]
        public string AwsSecretKey { get; set; }

        [CommandParameter(3, Description = "The name of the S3 bucket containing the matches.")]
        public string AwsBucketName { get; set; }

        [CommandParameter(4, Description = "The region the Aws S3 bucket is located in.")]
        public string AwsRegion { get; set; }

        [CommandParameter(5, Description = "The date for which the report will be generated. Format: yyyy-MM-dd")]
        public string Date { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(AwsRegion);
            var s3Client = new AmazonS3Client(AwsAccessKey, AwsSecretKey, regionEndpoint);
            var bucket = new AwsS3SummonerResultBucket(s3Client, AwsBucketName);

            var dailyReport = new Core.DailyReport(bucket);
            string report = await dailyReport.Create(Date);

            await console.Output.WriteLineAsync(report);
            await console.Input.ReadLineAsync();
        }
    }
}