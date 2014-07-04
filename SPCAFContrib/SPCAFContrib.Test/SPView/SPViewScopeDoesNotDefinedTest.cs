using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPView
{
    [TestClass]
    public class SPViewScopeDoesNotDefinedTest
    {
        [TestMethod]
        public void CheckSPViewScopeDoesNotDefined()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SPViewScopeDoesNotDefined() }));
        }
    }
}
