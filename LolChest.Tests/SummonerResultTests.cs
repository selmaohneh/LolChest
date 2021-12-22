using System;
using System.Collections.Generic;
using System.Linq;
using LolChest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LolChest.Tests
{
    [TestClass]
    public class SummonerResultTests
    {
        [TestMethod]
        public void IfNameIsHolgerHodensack_ReplaceWithDFFFiesoduck()
        {
            SummonerResult sut = SummonerResultTestFactory.CreateSummonerResult("Holger Hodensack");
            Assert.AreEqual("DFF Fiesoduck", sut.SummonerName);
        }

        [TestMethod]
        public void IfNameIsWhizzmaster_ReplaceWithDFFQuackerjack()
        {
            SummonerResult sut = SummonerResultTestFactory.CreateSummonerResult("Whizzmaster");
            Assert.AreEqual("DFF Quackerjack", sut.SummonerName);
        }

        [TestMethod]
        public void Won_CorrectPenalty()
        {
            SummonerResult sut = SummonerResultTestFactory.CreateSummonerResult("Whizzmaster");

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void Lost_CorrectPenalty()
        {
            SummonerResult sut = SummonerResultTestFactory.CreateSummonerResult("Whizzmaster", won: false);

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05 * 1.5), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void FirstBloodWon_CorrectPenalty()
        {
            SummonerResult sut = SummonerResultTestFactory.CreateSummonerResult("Whizzmaster", firstBloodParticipation: EFirstBloodParticipation.WonFirstBlood);

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05 - 0.05), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void CorrectToString()
        {
            SummonerResult sut = SummonerResultTestFactory.CreateSummonerResult();

            Assert.AreEqual("Homer Simpson (Orianna): 4/1/12 ==> 0,06€", sut.ToString());
        }

        [TestMethod]
        public void TestGetSummonersExtension()
        {
            SummonerResult sr1 = SummonerResultTestFactory.CreateSummonerResult("Homer");
            SummonerResult sr2 = SummonerResultTestFactory.CreateSummonerResult("Marge");
            SummonerResult sr3 = SummonerResultTestFactory.CreateSummonerResult("Homer");
            SummonerResult sr4 = SummonerResultTestFactory.CreateSummonerResult("Marge");

            var summonerResults = new List<SummonerResult>
            {
                sr1,
                sr2,
                sr3,
                sr4
            };

            var results = summonerResults.GetSummoners();
            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(summonerResults.Select(x => x.SummonerName).Contains("Homer"));
            Assert.IsTrue(summonerResults.Select(x => x.SummonerName).Contains("Marge"));
        }

        [TestMethod]
        public void TestOfExtension()
        {
            SummonerResult sr1 = SummonerResultTestFactory.CreateSummonerResult("Homer");
            SummonerResult sr2 = SummonerResultTestFactory.CreateSummonerResult("Marge");
            SummonerResult sr3 = SummonerResultTestFactory.CreateSummonerResult("Homer");
            SummonerResult sr4 = SummonerResultTestFactory.CreateSummonerResult("Marge");

            var summonerResults = new List<SummonerResult>
            {
                sr1,
                sr2,
                sr3,
                sr4
            };

            var results = summonerResults.Of("Marge").ToList();
            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.Select(x => x.SummonerName).All(x => x == "Marge"));
        }

        [TestMethod]
        public void TestGetLolChestSummonerResults()
        {
            SummonerResult sr1 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "1");
            SummonerResult sr2 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "2");
            SummonerResult sr3 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "3");

            SummonerResult sr4 = SummonerResultTestFactory.CreateSummonerResult("Marge", won: true, gameId: "1");

            SummonerResult sr5 = SummonerResultTestFactory.CreateSummonerResult("Marge", won: true, gameId: "2");

            SummonerResult sr6 = SummonerResultTestFactory.CreateSummonerResult("Bart", won: true, gameId: "3");
            SummonerResult sr7 = SummonerResultTestFactory.CreateSummonerResult("Bart", won: true, gameId: "4");

            var summonerResults = new List<SummonerResult>
            {
                sr1,
                sr2,
                sr3,
                sr4,
                sr5,
                sr6,
                sr7
            };

            var results = summonerResults.GetLolChestSummonerResults().ToList();
            Assert.AreEqual(6, results.Count);
            Assert.IsTrue(results.Contains(sr1));
            Assert.IsTrue(results.Contains(sr2));
            Assert.IsTrue(results.Contains(sr3));
            Assert.IsTrue(results.Contains(sr4));
            Assert.IsTrue(results.Contains(sr5));
            Assert.IsTrue(results.Contains(sr6));
        }

        [TestMethod]
        public void TestCountGames()
        {
            SummonerResult sr1 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "1");
            SummonerResult sr2 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "2");
            SummonerResult sr3 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "3");
            SummonerResult sr4 = SummonerResultTestFactory.CreateSummonerResult("Marge", won: true, gameId: "1");
            SummonerResult sr5 = SummonerResultTestFactory.CreateSummonerResult("Marge", won: true, gameId: "2");
            SummonerResult sr6 = SummonerResultTestFactory.CreateSummonerResult("Bart", won: true, gameId: "3");
            SummonerResult sr7 = SummonerResultTestFactory.CreateSummonerResult("Bart", won: true, gameId: "4");

            var summonerResults = new List<SummonerResult>
            {
                sr1,
                sr2,
                sr3,
                sr4,
                sr5,
                sr6,
                sr7
            };

            int games = summonerResults.CountGames();
            Assert.AreEqual(4, games);
        }

        [TestMethod]
        public void TestGetLolChestSummonerResults_Duplicate()
        {
            SummonerResult sr1 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "1");
            SummonerResult sr2 = SummonerResultTestFactory.CreateSummonerResult("Homer", won: true, gameId: "1");

            SummonerResult sr3 = SummonerResultTestFactory.CreateSummonerResult("Marge", won: true, gameId: "1");
            SummonerResult sr4 = SummonerResultTestFactory.CreateSummonerResult("Marge", won: true, gameId: "1");

            var summonerResults = new List<SummonerResult>
            {
                sr1,
                sr2,
                sr3,
                sr4,
            };

            var results = summonerResults.GetLolChestSummonerResults().ToList();
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Contains(sr1) || results.Contains(sr2));
            Assert.IsTrue(results.Contains(sr3) || results.Contains(sr4));
        }

        [TestMethod]
        public void TestPLotExtension()
        {
            SummonerResult sr1 = SummonerResultTestFactory.CreateSummonerResult(name: "Homer",
                                                                                championId: 61,
                                                                                won: false,
                                                                                gameId: "1",
                                                                                kills: 5,
                                                                                deaths: 3,
                                                                                assists: 12,
                                                                                durationMinutes: 78,
                                                                                creationYear: 2021,
                                                                                creationMonth: 3,
                                                                                creationDay: 2);

            SummonerResult sr2 = SummonerResultTestFactory.CreateSummonerResult(name: "Homer",
                                                                                championId: 62,
                                                                                won: true,
                                                                                gameId: "2",
                                                                                kills: 12,
                                                                                deaths: 4,
                                                                                assists: 21,
                                                                                durationMinutes: 18,
                                                                                creationYear: 2020,
                                                                                creationMonth: 7,
                                                                                creationDay: 13,
                                                                                firstBloodParticipation: EFirstBloodParticipation.WonFirstBlood);

            SummonerResult sr3 = SummonerResultTestFactory.CreateSummonerResult(name: "Marge",
                                                                                championId: 12,
                                                                                won: false,
                                                                                gameId: "1",
                                                                                kills: 1,
                                                                                deaths: 0,
                                                                                assists: 6,
                                                                                durationMinutes: 78,
                                                                                creationYear: 2021,
                                                                                creationMonth: 3,
                                                                                creationDay: 2);

            SummonerResult sr4 = SummonerResultTestFactory.CreateSummonerResult(name: "Bart",
                                                                                championId: 67,
                                                                                won: true,
                                                                                gameId: "2",
                                                                                kills: 0,
                                                                                deaths: 0,
                                                                                assists: 0,
                                                                                durationMinutes: 18,
                                                                                creationYear: 2020,
                                                                                creationMonth: 7,
                                                                                creationDay: 13);

            var summonerResults = new List<SummonerResult>
            {
                sr1,
                sr2,
                sr3,
                sr4,
            };

            string plot = summonerResults.Plot();
            Assert.AreEqual(Resources.ExpectedPlot, plot);
        }
    }
}