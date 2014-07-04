using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.UnsafeStringComporation.Tests
{
    [TestClass]
    public class AvoidSPObjectNameStringComparisonTest
    {
        [TestMethod]
        public void CheckAvoidSPObjectNameStringComparison()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new AvoidSPObjectNameStringComparison()}));
        }
    }
}
