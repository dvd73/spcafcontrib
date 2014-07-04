using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPDataSource
{
    [TestClass]
    public class SPDataSourceScopeDoesNotDefinedTest
    {
        [TestMethod]
        public void CheckSPDataSourceScopeDoesNotDefined()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SPDataSourceScopeDoesNotDefined() }));
        }
    }
}
