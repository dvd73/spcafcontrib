using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Control;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPDataSource
{
    [TestClass]
    public class AvoidUsingUpdatePanelInMasterPageTest
    {
        [TestMethod]
        public void CheckAvoidUsingUpdatePanelInMasterPage()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidUsingUpdatePanelInMasterPage() }));
        }
    }
}
