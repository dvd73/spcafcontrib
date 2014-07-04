using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPWebTest
{
    [TestClass]
    public class DoNotUseSPWebPropertiesTest
    {
        [TestMethod]
        public void CheckDoNotUseSPWebProperties()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotUseSPWebProperties() }));
        }
    }
}
