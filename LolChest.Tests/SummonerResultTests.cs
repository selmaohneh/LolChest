using System;
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
            var kda = new Kda(4, 1, 12);
            var duration = TimeSpan.FromMinutes(21);
            var gameCreation = new DateTime(2019, 5, 13);

            var sut = new SummonerResult("Holger Hodensack",
                                         61,
                                         kda,
                                         duration,
                                         true,
                                         "123456",
                                         gameCreation);

            Assert.AreEqual("DFF Fiesoduck", sut.SummonerName);
        }

        [TestMethod]
        public void IfNameIsWhizzmaster_ReplaceWithDFFQuackerjack()
        {
            var kda = new Kda(4, 1, 12);
            var duration = TimeSpan.FromMinutes(21);
            var gameCreation = new DateTime(2019, 5, 13);

            var sut = new SummonerResult("Whizzmaster",
                                         61,
                                         kda,
                                         duration,
                                         true,
                                         "123456",
                                         gameCreation);

            Assert.AreEqual("DFF Quackerjack", sut.SummonerName);
        }

        [TestMethod]
        public void Won_CorrectPenalty()
        {
            var kda = new Kda(4, 1, 12);
            var duration = TimeSpan.FromMinutes(21);
            var gameCreation = new DateTime(2019, 5, 13);

            var sut = new SummonerResult("Whizzmaster",
                                         61,
                                         kda,
                                         duration,
                                         true,
                                         "123456",
                                         gameCreation);

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void Lost_CorrectPenalty()
        {
            var kda = new Kda(4, 1, 12);
            var duration = TimeSpan.FromMinutes(21);
            var gameCreation = new DateTime(2019, 5, 13);

            var sut = new SummonerResult("Whizzmaster",
                                         61,
                                         kda,
                                         duration,
                                         false,
                                         "123456",
                                         gameCreation);

            decimal expectedResult = Math.Round((decimal)(1.0 / (4 + 12 + 1.0) * 21.0 * 0.05 * 1.5), 2);
            Assert.AreEqual(expectedResult, sut.GetPenalty());
        }

        [TestMethod]
        public void CorrectToString()
        {
            var kda = new Kda(4, 1, 12);
            var duration = TimeSpan.FromMinutes(21);
            var gameCreation = new DateTime(2019, 5, 13);

            var sut = new SummonerResult("Homer Simpson",
                                         61,
                                         kda,
                                         duration,
                                         true,
                                         "123456",
                                         gameCreation);

            Assert.AreEqual("Homer Simpson (Orianna): 4/1/12 ==> 0,06€", sut.ToString());
        }
    }
}