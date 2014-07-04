using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPContext
{
    [TestClass]
    public class AvoidUsingSPContextOutsideOfWebContextTest
    {
        [TestMethod]
        public void CheckAvoidUsingSPContextOutsideOfWebContext()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new AvoidUsingSPContextOutsideOfWebContext()}));
        }
    }
}
