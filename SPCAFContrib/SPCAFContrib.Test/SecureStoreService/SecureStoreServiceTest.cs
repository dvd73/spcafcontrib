using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPCAFContrib.Test.SecureStoreService
{
    [TestClass]
    public class SecureStoreServiceTest 
    {
        [TestMethod]
        public void UseSecureStoreService()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new UseSecureStoreService()}));
        }
    }
}
