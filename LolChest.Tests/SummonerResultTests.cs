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
            SummonerResult sut = CreateSummonerResult("Holger Hodensack");
            Assert.AreEqual("DFF Fiesoduck", sut.SummonerName);
        }

        [TestMethod]
        public void IfNameIsWhizzmaster_ReplaceWithDFFQuackerjack()
        {
            SummonerResult sut = CreateSummonerResult("Whizzmaster");
            Assert.AreEqual("DFF Quackerjack", sut.SummonerName);
        }

        [TestMethod]
        public void Won_CorrectPenalty()
        {
            SummonerResult sut = CreateSummonerResult("Whizzmaster");

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void Lost_CorrectPenalty()
        {
            SummonerResult sut = CreateSummonerResult("Whizzmaster", false);

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05 * 1.5), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void CorrectToString()
        {
            SummonerResult sut = CreateSummonerResult("Homer Simpson");

            Assert.AreEqual("Homer Simpson (Orianna): 4/1/12 ==> 0,06€", sut.ToString());
        }

        [TestMethod]
        public void TestGetSummonersExtension()
        {
            var sr1 = CreateSummonerResult("Homer");
            var sr2 = CreateSummonerResult("Marge");
            var sr3 = CreateSummonerResult("Homer");
            var sr4 = CreateSummonerResult("Marge");

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
            var sr1 = CreateSummonerResult("Homer");
            var sr2 = CreateSummonerResult("Marge");
            var sr3 = CreateSummonerResult("Homer");
            var sr4 = CreateSummonerResult("Marge");

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
            var sr1 = CreateSummonerResult("Homer", true, "1");
            var sr2 = CreateSummonerResult("Homer", true, "2");
            var sr3 = CreateSummonerResult("Homer", true, "3");

            var sr4 = CreateSummonerResult("Marge", true, "1");

            var sr5 = CreateSummonerResult("Marge", true, "2");

            var sr6 = CreateSummonerResult("Bart", true, "3");
            var sr7 = CreateSummonerResult("Bart", true, "4");

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
        public void TestGetLolChestSummonerResults_Duplicate()
        {
            var sr1 = CreateSummonerResult("Homer", true, "1");
            var sr2 = CreateSummonerResult("Homer", true, "1");

            var sr3 = CreateSummonerResult("Marge", true, "1");
            var sr4 = CreateSummonerResult("Marge", true, "1");

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

        private SummonerResult CreateSummonerResult(string name)
        {
            return CreateSummonerResult(name, true);
        }

        private SummonerResult CreateSummonerResult(string name, bool won, string gameId = "123456")
        {
            var kda = new Kda(4, 1, 12);
            var duration = TimeSpan.FromMinutes(21);
            var gameCreation = new DateTime(2019, 5, 13);

            return new SummonerResult(name,
                                      61,
                                      kda,
                                      duration,
                                      won,
                                      gameId,
                                      gameCreation);
        }
    }
}