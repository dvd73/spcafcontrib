using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Logging
{
    [TestClass]
    public class NoCustomLoggingTest
    {
        [TestMethod]
        public void CheckNoCustomLoggingTest()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new [] { new NoCustomLogging() }));
        }
    }
}
