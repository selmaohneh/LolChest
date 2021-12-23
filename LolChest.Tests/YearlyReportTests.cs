using System.Collections.Generic;
using System.Threading.Tasks;
using LolChest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LolChest.Tests
{
    [TestClass]
    public class YearlyReportTests
    {
        [TestMethod]
        public async Task TestYearlyReport()
        {
            SummonerResult summonerResult1 = SummonerResultTestFactory.CreateSummonerResult("Cello", 61, true, "1", 4,2,12, 39, 2021, 12, 24, EFirstBloodParticipation.NoParticipation);
            SummonerResult summonerResult2 = SummonerResultTestFactory.CreateSummonerResult("Cello", 62, false, "2", 2,1,24, 42, 2021, 9, 12, EFirstBloodParticipation.WonFirstBlood);
            SummonerResult summonerResult3 = SummonerResultTestFactory.CreateSummonerResult("Cello", 63, true, "3", 4,8,8, 15, 2021, 7, 6, EFirstBloodParticipation.NotYetTracked);
            SummonerResult summonerResult4 = SummonerResultTestFactory.CreateSummonerResult("Whizzy", 12, true, "1", 13,13,34, 39, 2021, 12, 24, EFirstBloodParticipation.WonFirstBlood);
            SummonerResult summonerResult5 = SummonerResultTestFactory.CreateSummonerResult("Whizzy", 11, false, "2", 4,4,10, 42, 2021, 9, 12, EFirstBloodParticipation.WonFirstBlood);
            SummonerResult summonerResult6 = SummonerResultTestFactory.CreateSummonerResult("Whizzy", 10, true, "3", 4,0,15, 15, 2021, 7, 6, EFirstBloodParticipation.NoParticipation);
            SummonerResult summonerResult7 = SummonerResultTestFactory.CreateSummonerResult("Whizzy", 22, true, "4", 4,0,15, 15, 2021, 7, 6, EFirstBloodParticipation.NoParticipation);
            SummonerResult summonerResult8 = SummonerResultTestFactory.CreateSummonerResult("Benni", 9, true, "1", 5,5,24, 39, 2021, 9, 24, EFirstBloodParticipation.NoParticipation);
            SummonerResult summonerResult9 = SummonerResultTestFactory.CreateSummonerResult("Benni", 8, false, "2", 0,0,11, 42, 2021, 7, 12, EFirstBloodParticipation.NoParticipation);
            SummonerResult summonerResult10 = SummonerResultTestFactory.CreateSummonerResult("Benni", 23, true, "4", 4,0,15, 15, 2021, 7, 6, EFirstBloodParticipation.NoParticipation);

            var summonerResults = new List<SummonerResult>
            {
                summonerResult1,
                summonerResult2,
                summonerResult3,
                summonerResult4,
                summonerResult5,
                summonerResult6,
                summonerResult7,
                summonerResult8,
                summonerResult9,
                summonerResult10
            };

            var bucket = new Mock<ISummonerResultBucket>();

            bucket.Setup(x => x.GetForYear("2021")).ReturnsAsync(summonerResults);

            var sut = new YearlyReport(bucket.Object);

            string report = await sut.Create("2021");

            Assert.Inconclusive();
        }
    }
}
