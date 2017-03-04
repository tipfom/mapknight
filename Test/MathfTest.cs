using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mapKnight.Core;

namespace mapKnight.Test {
    [TestClass]
    public class MathfTest {
        [TestMethod]
        public void TestSin ( ) {
            for (float i = -420; i <= 420; i += 0.25f) {
                Assert.AreEqual(Math.Round(Math.Sin(i * Math.PI / 180f), 3), Math.Round(Mathf.Sin(i), 3));
            }
        }

        [TestMethod]
        public void TestCos ( ) {
            for (float i = -420; i <= 420; i += 0.25f) {
                Assert.AreEqual(Math.Round(Math.Cos(i * Math.PI / 180f), 3), Math.Round(Mathf.Cos(i), 3));
            }
        }

        [TestMethod]
        public void TestSqrt ( ) {
            for(int i = 1; i < int.MaxValue && i > 0; i += 1) {
                // error is less then 0.5%
                Assert.IsTrue(Math.Abs(Mathf.Sqrt(i) / Math.Sqrt(i) - 1) <= 0.005);
            }
        }

        [TestMethod]
        public void TestPow ( ) {
            for (int i = 0; i < 100; i++) {
                Assert.AreEqual((float)Math.Pow(i, 0), Mathf.Pow(i, 0));
                Assert.AreEqual((float)Math.Pow(i, 1), Mathf.Pow(i, 1));
                Assert.AreEqual((float)Math.Pow(i, 2), Mathf.Pow(i, 2));
                Assert.AreEqual((float)Math.Pow(i, 91), Mathf.Pow(i, 91));
            }
        }
    }
}
