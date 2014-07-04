using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPSecurity
{
    [TestClass]
    public class OutOfContextRWEPTest
    {
        [TestMethod]
        public void CheckOutOfContextRWEP()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new OutOfContextRWEP() }));
        }
    }
}
