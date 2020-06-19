using RiotSharp.Endpoints.MatchEndpoint;

namespace LolChest
{
    public class Kda
    {
        public int Kills { get; }
        public int Deaths { get; }
        public int Assists { get; }

        public Kda(ParticipantStats stats)
        {
            Kills = (int) stats.Kills;
            Deaths = (int) stats.Deaths;
            Assists = (int) stats.Assists;
        }
    }
}