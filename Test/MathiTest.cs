using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mapKnight.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mapKnight.Test {
    [TestClass]
   public class MathiTest {
        [TestMethod]
        public void TestCeil ( ) {
            for(float i = -10; i < 10; i += 0.1f) {
                Assert.AreEqual(Math.Ceiling(i), Mathi.Ceil(i));
            }
        }

        [TestMethod]
        public void TestFloor ( ) {
            for (float i = -10; i < 10; i += 0.1f) {
                Assert.AreEqual(Math.Floor(i), Mathi.Floor(i));
            }
        }
    }
}
