using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Metrics.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Metrics
{
    [TestClass]
    public class NumberOfBreakInheritanceTest
    {
        [TestMethod]
        public void CountNumberOfBreakInheritance()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new NumberOfBreakInheritance()}));
        }
    }
}
