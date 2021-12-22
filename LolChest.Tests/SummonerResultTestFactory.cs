using System;
using LolChest.Core;

namespace LolChest.Tests
{
    internal static class SummonerResultTestFactory
    {
        internal static SummonerResult CreateSummonerResult(string name = "Homer Simpson",
                                                            int championId = 61,
                                                            bool won = true,
                                                            string gameId = "123456",
                                                            int kills = 4,
                                                            int deaths = 1,
                                                            int assists = 12,
                                                            int durationMinutes = 21,
                                                            int creationYear = 2019,
                                                            int creationMonth = 5,
                                                            int creationDay = 13,
                                                            EFirstBloodParticipation firstBloodParticipation = EFirstBloodParticipation.NoParticipation)
        {
            return new SummonerResult(name,
                                      championId,
                                      new Kda(kills, deaths, assists),
                                      TimeSpan.FromMinutes(durationMinutes),
                                      won,
                                      gameId,
                                      new DateTime(creationYear, creationMonth, creationDay),
                                      firstBloodParticipation);
        }
    }
}