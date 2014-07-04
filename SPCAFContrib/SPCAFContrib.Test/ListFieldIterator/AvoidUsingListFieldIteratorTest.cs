using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.ListFieldIterator
{
    [TestClass]
    public class AvoidUsingListFieldIteratorTest
    {
        [TestMethod]
        public void CheckAvoidUsingListFieldIterator()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidUsingListFieldIterator() }));
        }
    }
}
