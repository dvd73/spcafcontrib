using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Other;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Solution
{
    [TestClass]
    public class UnresolvedTokenAssemblyFullNameTest
    {
        [TestMethod]
        public void CheckUnresolvedTokenAssemblyFullName()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new UnresolvedTokenAssemblyFullName() }));
        }
    }
}
