using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.MasterPage;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPDataSource
{
    [TestClass]
    public class SPDataSourceScopeDoesNotDefinedInMasterPageTest
    {
        [TestMethod]
        public void CheckSPDataSourceScopeDoesNotDefinedInMasterPage()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SPDataSourceScopeDoesNotDefinedInMasterPage() }));
        }
    }
}
