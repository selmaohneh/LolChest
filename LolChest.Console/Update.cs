﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Camille.Enums;
using Camille.RiotGames;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LolChest.Core;

namespace LolChest.Console
{
    [Command("update", Description = "Adds common recent matches to the LolChest.")]
    public class Update : ICommand
    {
        [CommandParameter(0, Description = "The key for the Riot games API.")]
        public string RiotGamesApiKey { get; set; }

        [CommandParameter(1, Description = "The access key for Aws.")]
        public string AwsAccessKey { get; set; }

        [CommandParameter(2, Description = "The secret key for Aws.")]
        public string AwsSecretKey { get; set; }

        [CommandParameter(3, Description = "The name of the S3 bucket containing the matches.")]
        public string AwsBucketName { get; set; }

        [CommandParameter(4, Description = "The region the Aws S3 bucket is located in.")]
        public string AwsRegion { get; set; }

        [CommandParameter(5, Description = "The platform route of the summoners.")]
        public PlatformRoute PlatformRoute { get; set; }

        [CommandParameter(6, Description = "The regional route of the summoners.")]
        public RegionalRoute RegionalRoute { get; set; }

        [CommandParameter(7, Description = "The in-game summoner names.")]
        public IEnumerable<string> SummonerNames { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var riotGamesApi = RiotGamesApi.NewInstance(RiotGamesApiKey);

            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(AwsRegion);
            var s3Client = new AmazonS3Client(AwsAccessKey, AwsSecretKey, regionEndpoint);
            var bucket = new AwsS3SummonerResultBucket(s3Client, AwsBucketName);

            var update = new Core.Update(riotGamesApi, bucket);

            await update.Execute(PlatformRoute, RegionalRoute, SummonerNames);
        }
    }
}