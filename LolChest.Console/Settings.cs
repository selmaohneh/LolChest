using System.Collections.Generic;

namespace LolChest.Console
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public IEnumerable<string> SummonerNames { get; set; }
    }
}