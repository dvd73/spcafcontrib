using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.ListInstance
{
    [TestClass]
    public class UniqueListInstanceUrlTest
    {
        [TestMethod]
        public void CheckUniqueListInstanceUrl()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new UniqueListInstanceUrl()}));
        }
    }
}
