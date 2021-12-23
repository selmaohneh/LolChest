namespace LolChest.Core
{
    /// <summary>
    /// KDA is short for "kills, deaths, assists".
    /// Contains the amount of kills, deaths and assists a summoner got in a
    /// single match.
    /// </summary>
    public class Kda
    {
        public int Kills { get; }
        public int Deaths { get; }
        public int Assists { get; }

        public Kda(int kills, int deaths, int assists)
        {
            Kills = kills;
            Deaths = deaths;
            Assists = assists;
        }

        public override string ToString()
        {
            return $"{Kills}/{Deaths}/{Assists}";
        }

        public double GetSingleDigitKda()
        {
            int ka = Kills + Assists;

            if (Deaths > 0)
            {
                return ka / (double)Deaths;
            }

            return ka;
        }
    }
}