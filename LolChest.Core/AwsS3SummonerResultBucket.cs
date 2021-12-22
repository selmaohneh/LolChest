using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace LolChest.Core
{
    public class AwsS3SummonerResultBucket : ISummonerResultBucket
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public AwsS3SummonerResultBucket(AmazonS3Client s3Client, string bucketName)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
        }

        public async Task<IEnumerable<SummonerResult>> GetForDay(string day)
        {
            return await GetWithPrefix(day);
        }

        public async Task<IEnumerable<SummonerResult>> GetForMonth(string month)
        {
            return await GetWithPrefix(month);
        }

        public async Task<IEnumerable<SummonerResult>> GetForYear(string year)
        {
            return await GetWithPrefix(year);
        }

        public async Task Save(SummonerResult summonerResult)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key =
                    $"{summonerResult.GameCreation:yyyy}-{summonerResult.GameCreation:MM}-{summonerResult.GameCreation:dd}-{summonerResult.GameId}-{summonerResult.SummonerName}.lol",
                ContentBody = JsonConvert.SerializeObject(summonerResult, Formatting.Indented)
            };

            await _s3Client.PutObjectAsync(putRequest);
        }

        private async Task<IEnumerable<SummonerResult>> GetWithPrefix(string prefix)
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = prefix
            };

            ListObjectsV2Response listResponse = await _s3Client.ListObjectsV2Async(listRequest);

            var summonerResults = new List<SummonerResult>();

            foreach (S3Object s3Object in listResponse.S3Objects)
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = s3Object.Key,
                };

                string json;

                using (GetObjectResponse getResponse = await _s3Client.GetObjectAsync(getRequest))
                using (Stream responseStream = getResponse.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    json = await reader.ReadToEndAsync();
                }

                var summonerResult = JsonConvert.DeserializeObject<SummonerResult>(json);
                summonerResults.Add(summonerResult);
            }

            return summonerResults;
        }
    }
}