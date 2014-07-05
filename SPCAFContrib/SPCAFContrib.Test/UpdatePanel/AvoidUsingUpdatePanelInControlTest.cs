using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Control;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPDataSource
{
    [TestClass]
    public class AvoidUsingUpdatePanelInControlTest
    {
        [TestMethod]
        public void CheckAvoidUsingUpdatePanelInControl()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidUsingUpdatePanelInControl() }));
        }
    }
}
