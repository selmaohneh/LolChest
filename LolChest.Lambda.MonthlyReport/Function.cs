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

namespace LolChest.Lambda.MonthlyReport
{
    public class Function
    {
        public async Task<string> FunctionHandler(string date)
        {
            if (date == "automatic")
            {
                DateTime today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                DateTime lastDayOfLastMonth = firstDayOfMonth - TimeSpan.FromDays(1);

                date = lastDayOfLastMonth.ToString("yyyy-MM");
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

            var monthlyReport = new LolChest.Core.MonthlyReport(bucket);

            string report = await monthlyReport.Create(date);

            if (parsedEmailAddresses != null && parsedEmailAddresses.Any() && report != null)
            {
                await emailSender.SendSummaryAsEmail($"LolChest: Session results {date}", report, parsedEmailAddresses);
            }

            return report;
        }
    }
}