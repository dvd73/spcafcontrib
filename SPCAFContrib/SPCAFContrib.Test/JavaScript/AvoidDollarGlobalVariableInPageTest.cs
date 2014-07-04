using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.MasterPage;
using SPCAFContrib.Rules.Page;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidDollarGlobalVariableInPageTest
    {
        [TestMethod]
        public void CheckAvoidDollarGlobalVariableInPage()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidDollarGlobalVariableInPage() }));
            //Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidDollarGlobalVariableInMasterPage() }));
        }
    }
}
