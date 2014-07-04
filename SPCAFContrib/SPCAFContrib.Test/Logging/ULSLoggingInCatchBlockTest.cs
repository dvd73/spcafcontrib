using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Logging
{
    [TestClass]
    public class ULSLoggingInCatchBlockTest
    {
        [TestMethod]
        public void CheckULSLoggingInCatchBlock()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new [] { new ULSLoggingInCatchBlock()}));
        }
    }
}
