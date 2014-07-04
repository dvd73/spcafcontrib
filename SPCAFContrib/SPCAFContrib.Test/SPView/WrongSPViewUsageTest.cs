using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPView
{
    [TestClass]
    public class WrongSPViewUsageTest
    {
        [TestMethod]
        public void CheckWrongSPViewUsage()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new WrongSPViewUpdate() }));
        }
    }
}
