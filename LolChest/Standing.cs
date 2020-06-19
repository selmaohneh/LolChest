using System.Collections.Generic;
using System.Linq;

namespace LolChest
{
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
    }
}