using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPWebTest
{
    [TestClass]
    public class DoNotDisposePersonalSiteWebTest
    {
        [TestMethod]
        public void CheckDoNotDisposePersonalSiteWeb()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotDisposePersonalSiteWeb() }));
        }
    }
}
