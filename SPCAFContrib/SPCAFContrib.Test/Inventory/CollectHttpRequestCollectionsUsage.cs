using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Inventory;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Inventory
{
    [TestClass]
    public class CollectHttpRequestCollectionsUsage
    {
        #region tests

        [TestMethod]
        public void CheckHttpRequestCollectionsUsage()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new HttpRequestCollectionsUsage() }));
        }

        #endregion
    }
}
