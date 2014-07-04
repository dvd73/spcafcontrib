using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Experimental.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.WebParts
{
    [TestClass]
    public class AvoidAllUsersWebPartInModulesTest
    {
        [TestMethod]
        public void CheckAvoidAllUsersWebPartInModules()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidAllUsersWebPartInModules() }));
        }
    }
}
