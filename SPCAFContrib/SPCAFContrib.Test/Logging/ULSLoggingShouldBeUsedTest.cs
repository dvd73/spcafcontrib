using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Logging
{
    [TestClass]
    public class ULSLoggingShouldBeUsedTest
    {
        [TestMethod]
        public void CheckULSLoggingShouldBeUsed()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new [] { new ULSLoggingShouldBeUsed() }));
        }
    }
}
