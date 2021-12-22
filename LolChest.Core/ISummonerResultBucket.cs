using System.Collections.Generic;
using System.Threading.Tasks;

namespace LolChest.Core
{
    public interface ISummonerResultBucket
    {
        Task<IEnumerable<SummonerResult>> GetForDay(string day);
        Task<IEnumerable<SummonerResult>> GetForMonth(string month);
        Task Save(SummonerResult summonerResult);
        Task<IEnumerable<SummonerResult>> GetForYear(string year);
    }
}