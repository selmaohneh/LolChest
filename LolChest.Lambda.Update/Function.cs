using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Camille.Enums;
using Camille.RiotGames;
using LolChest.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LolChest.Lambda.Update
{
    public class Function
    {
        public async Task<string> FunctionHandler()
        {
            string riotGamesApiKey = Environment.GetEnvironmentVariable("RiotGamesApiKey");
            string awsAccessKey = Environment.GetEnvironmentVariable("AwsAccessKey");
            string awsSecretKey = Environment.GetEnvironmentVariable("AwsSecretKey");
            string awsRegion = Environment.GetEnvironmentVariable("AwsRegion");
            string awsBucketName = Environment.GetEnvironmentVariable("AwsBucketName");
            string platformRoute = Environment.GetEnvironmentVariable("PlatformRoute");
            string regionalRoute = Environment.GetEnvironmentVariable("RegionalRoute");
            string summonerNames = Environment.GetEnvironmentVariable("SummonerNames");

            Enum.TryParse(platformRoute, out PlatformRoute parsedPlatformRoute);
            Enum.TryParse(regionalRoute, out RegionalRoute parsedRegionalRoute);
            string[] parsedSummonerNames = summonerNames.Split(',');

            var riotGamesApi = RiotGamesApi.NewInstance(riotGamesApiKey);

            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(awsRegion);
            var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, regionEndpoint);
            var bucket = new AwsS3SummonerResultBucket(s3Client, awsBucketName);

            var update = new LolChest.Core.Update(riotGamesApi, bucket);

            string lastGameCreation = await update.Execute(parsedPlatformRoute, parsedRegionalRoute, parsedSummonerNames);

            return lastGameCreation;
        }
    }
}