using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Control;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPDataSource
{
    [TestClass]
    public class SPDataSourceScopeDoesNotDefinedInControlTest
    {
        [TestMethod]
        public void CheckSPDataSourceScopeDoesNotDefinedInControl()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SPDataSourceScopeDoesNotDefinedInControl() }));
        }
    }
}
