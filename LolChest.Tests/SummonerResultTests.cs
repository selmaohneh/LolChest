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
    }
}