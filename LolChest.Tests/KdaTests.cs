using LolChest.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LolChest.Tests
{
    [TestClass]
    public class KdaTests
    {
        [TestMethod]
        public void TestToString()
        {
            var sut = new Kda(42, 21, 99);
            string result = sut.ToString();

            Assert.AreEqual("42/21/99", result);
        }

        [TestMethod]
        public void TestSingleDigitKda()
        {
            var sut = new Kda(42, 21, 99);
            double result = sut.GetSingleDigitKda();

            Assert.AreEqual((42.0 + 99.0) / 21.0, result);
        }

        [TestMethod]
        public void TestSingleDigitKda_NoDeaths()
        {
            var sut = new Kda(42, 0, 99);
            double result = sut.GetSingleDigitKda();

            Assert.AreEqual(42.0 + 99.0, result);
        }
    }
}