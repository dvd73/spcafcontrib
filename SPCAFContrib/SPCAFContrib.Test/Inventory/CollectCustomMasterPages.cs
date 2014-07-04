using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Inventory;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Test.SearchString;

namespace SPCAFContrib.Test.Inventory
{
    [TestClass]
    public class CollectCustomMasterPages
    {
        #region tests

        [TestMethod]
        public void GetCustomMasterPages()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new CustomMasterPages()}));
        }

        #endregion
    }
}
