using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mapKnight.Core;

namespace mapKnight.Test {
    [TestClass]
    public class MathfTest {
        [TestMethod]
        public void TestSinTableValues ( ) {
            for (float i = -420; i <= 420; i += 0.25f) {
                Assert.AreEqual(Math.Round(Math.Sin(i * Math.PI / 180f), 3), Math.Round(Mathf.Sin(i), 3));
            }
        }

        [TestMethod]
        public void TestCosTableValues ( ) {
            for (float i = -420; i <= 420; i += 0.25f) {
                Assert.AreEqual(Math.Round(Math.Cos(i * Math.PI / 180f), 3), Math.Round(Mathf.Cos(i), 3));
            }
        }
    }
}
