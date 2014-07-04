using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPQuery
{
    [TestClass]
    public class DoNotChangeSPPersistedObjectTest
    {
        [TestMethod]
        public void CheckDoNotChangeSPPersistedObject()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotChangeSPPersistedObject() }));
        }
    }
}
