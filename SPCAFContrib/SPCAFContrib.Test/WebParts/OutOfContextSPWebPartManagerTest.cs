using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPSecurity
{
    [TestClass]
    public class OutOfContextSPWebPartManagerTest
    {
        [TestMethod]
        public void CheckOutOfContextSPWebPartManager()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new OutOfContextSPWebPartManager() }));
        }
    }
}
