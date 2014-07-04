using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPMonitoredScope
{
    [TestClass]
    public class SPMonitoredScopeShouldBeUsedTest
    {
        [TestMethod]
        public void CheckSPMonitoredScopeShouldBeUsed()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SPMonitoredScopeShouldBeUsed() }));
        }
    }
}
