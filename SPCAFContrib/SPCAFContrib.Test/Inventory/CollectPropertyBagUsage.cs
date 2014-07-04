using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Inventory;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Inventory
{
    [TestClass]
    public class CollectPropertyBagUsage
    {
        [TestMethod]
        public void GetPropertyBagUsages()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new PropertyBagUsage()}));
        }
    }
}
