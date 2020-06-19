using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest
{
    public class Standings
    {
        private readonly LolChestConfig _lolChestConfig;
        private readonly CloudTables _cloudTables;
        private List<LolChestEntry> _lolChestEntries;
        private List<Standing> _standings;
        private int _month;
        private int _year;

        public Standings(LolChestConfig lolChestConfig, CloudTables cloudTables)
        {
            _lolChestConfig = lolChestConfig;
            _cloudTables = cloudTables;
        }

        [FunctionName("Standings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "{month:int}/{year:int}")]
            HttpRequest req,
            int month,
            int year,
            ILogger log)
        {
            _year = year;
            _month = month;
            var orderedMatches = await GetOrderedMatches();

            CreateLolChestEntries(orderedMatches);
            CreateStandings();

            return new OkObjectResult(ObjectDumper.Dump(_standings) + ObjectDumper.Dump(_lolChestEntries));
        }

        private async Task<List<MatchEntity>> GetOrderedMatches()
        {
            var table = await _cloudTables.Get("registeredmatches");

            var query = CreateQuery();
            var matches = await table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
            var orderedMatches = matches.Results.OrderByDescending(x => x.GameCreation);
            return orderedMatches.ToList();
        }

        private TableQuery<MatchEntity> CreateQuery()
        {
            var firstDayOfMonth = new DateTime(_year, _month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var startFilter = TableQuery.GenerateFilterConditionForDate(
                "GameCreation",
                QueryComparisons.GreaterThanOrEqual,
                firstDayOfMonth);

            var endFilter = TableQuery.GenerateFilterConditionForDate(
                "GameCreation",
                QueryComparisons.LessThanOrEqual,
                lastDayOfMonth);

            var finalFilter = TableQuery.CombineFilters(
                startFilter,
                TableOperators.And,
                endFilter);

            var query = new TableQuery<MatchEntity>().Where(finalFilter);
            return query;
        }

        private void CreateLolChestEntries(List<MatchEntity> orderedMatches)
        {
            var lolChestEntries = new List<LolChestEntry>();
            foreach (var matchEntity in orderedMatches)
            {
                var match = JsonConvert.DeserializeObject<Match>(matchEntity.MatchJson);
                var lolChestEntry = new LolChestEntry(match, _lolChestConfig.SummonerNames);
                lolChestEntries.Add(lolChestEntry);
            }

            _lolChestEntries = lolChestEntries;
        }

        private void CreateStandings()
        {
            var standings = new List<Standing>();
            foreach (var summonerName in _lolChestConfig.SummonerNames)
            {
                var standing = new Standing(_lolChestEntries, summonerName);
                standings.Add(standing);
            }

            _standings = standings;
        }
    }
}