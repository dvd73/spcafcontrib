using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPQuery
{
    [TestClass]
    public class SPQueryScopeDoesNotDefinedTest
    {
        [TestMethod]
        public void CheckSPQueryScopeDoesNotDefined()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SPQueryScopeDoesNotDefined() }));
        }
    }
}
