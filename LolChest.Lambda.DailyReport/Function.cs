using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SimpleEmailV2;
using LolChest.Core;
using LolChest.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LolChest.Lambda.DailyReport
{
    public class Function
    {
        public async Task<string> FunctionHandler(string date)
        {
            if (date == "automatic")
            {
                DateTime yesterday = DateTime.Today - TimeSpan.FromDays(1);
                date = yesterday.ToString("yyyy-MM-dd");
            }

            string awsAccessKey = Environment.GetEnvironmentVariable("AwsAccessKey");
            string awsSecretKey = Environment.GetEnvironmentVariable("AwsSecretKey");
            string awsBucketName = Environment.GetEnvironmentVariable("AwsBucketName");
            string awsRegion = Environment.GetEnvironmentVariable("AwsRegion");
            string emailAddresses = Environment.GetEnvironmentVariable("EmailAddresses");

            var parsedEmailAddresses = emailAddresses?.Split(',').ToList();

            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(awsRegion);
            var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, regionEndpoint);
            var bucket = new AwsS3SummonerResultBucket(s3Client, awsBucketName);

            var sesClient = new AmazonSimpleEmailServiceV2Client(awsAccessKey, awsSecretKey, regionEndpoint);
            var emailSender = new EmailSender(sesClient);

            var dailyReport = new LolChest.Core.DailyReport(bucket);

            string report = await dailyReport.Create(date);

            if (parsedEmailAddresses != null && parsedEmailAddresses.Any() && report != null)
            {
                await emailSender.SendSummaryAsEmail($"LolChest: Session results {date}", report, parsedEmailAddresses);
            }

            return report;
        }
    }
}