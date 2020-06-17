using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using PowerArgs;
using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest.Console
{
    public class Program
    {
        private static List<LolChestEntry> _lolChestEntries;
        private static IEnumerable<Standing> _standings;
        private static LolChestArgs _lolChestArgs;
        private static Settings _settings;

        public static async Task Main(string[] args)
        {
            System.Console.OutputEncoding = Encoding.UTF8;

            _lolChestArgs = Args.Parse<LolChestArgs>(args);
            await LoadSettingsFromFile();

            var orderedMatches = await GetOrderedMatches();

            CreateLolChestEntries(orderedMatches);
            PlotLolChestEntries();

            CreateStandings();
            PlotStandings();

            System.Console.WriteLine();
            System.Console.Write("Press any key to exit... ");
            System.Console.ReadKey();
        }

        private static void PlotStandings()
        {
            System.Console.WriteLine(
                $"Standings for {_lolChestArgs.Month:00}.{_lolChestArgs.Year:0000}: ");

            foreach (var standing in _standings)
            {
                System.Console.WriteLine(standing);
            }
        }

        private static void CreateStandings()
        {
            var standings = new List<Standing>();
            foreach (var summonerName in _settings.SummonerNames)
            {
                var standing = new Standing(_lolChestEntries, summonerName);
                standings.Add(standing);
            }

            _standings = standings;
        }


        public class Standing
        {
            public string SummonerName { get; }
            public decimal SummedPenalty { get; }

            public Standing(IEnumerable<LolChestEntry> lolChestEntries, string summonerName)
            {
                SummonerName = summonerName;
                SummedPenalty = 0.0m;

                foreach (var lolChestEntry in lolChestEntries)
                {
                    SummedPenalty += lolChestEntry.LolChestStats.Single(x => x.SummonerName == summonerName).Penalty;
                }
            }

            public override string ToString()
            {
                return $"{SummonerName}: {SummedPenalty:0.00}€";
            }
        }

        private static void PlotLolChestEntries()
        {
            foreach (var lolChestEntry in _lolChestEntries)
            {
                System.Console.WriteLine(lolChestEntry);
            }
        }

        private static void CreateLolChestEntries(List<MatchEntity> orderedMatches)
        {
            var lolChestEntries = new List<LolChestEntry>();
            foreach (var matchEntity in orderedMatches)
            {
                var match = JsonConvert.DeserializeObject<Match>(matchEntity.MatchJson);
                var lolChestEntry = new LolChestEntry(match, _settings.SummonerNames);
                lolChestEntries.Add(lolChestEntry);
            }

            _lolChestEntries = lolChestEntries;
        }

        private static async Task LoadSettingsFromFile()
        {
            _settings = JsonConvert.DeserializeObject<Settings>(await File.ReadAllTextAsync("local.settings.json"));
        }

        private static async Task<List<MatchEntity>> GetOrderedMatches()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("registeredmatches");

            var query = CreateQuery();
            var matches = await table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
            var orderedMatches = matches.Results.OrderBy(x => x.GameCreation);
            return orderedMatches.ToList();
        }

        private static TableQuery<MatchEntity> CreateQuery()
        {
            var firstDayOfMonth = new DateTime(_lolChestArgs.Year, _lolChestArgs.Month, 1);
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
    }
}