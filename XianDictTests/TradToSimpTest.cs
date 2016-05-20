using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XianDict;

namespace XianDictTests
{
    [TestClass]
    public class TradToSimpTest
    {
        [TestMethod]
        public void Convert_Test()
        {
            Assert.AreEqual("马", TradToSimp.Convert("馬").First());
            Assert.AreEqual("现代汉语", TradToSimp.Convert("現代漢語").First());
        }
    }
}
