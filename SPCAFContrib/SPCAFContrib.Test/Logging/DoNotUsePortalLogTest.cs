using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Logging
{
    [TestClass]
    public class DoNotUsePortalLogTest
    {
        [TestMethod]
        public void CheckDoNotUsePortalLog()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotUsePortalLog() }));
        }
    }
}
