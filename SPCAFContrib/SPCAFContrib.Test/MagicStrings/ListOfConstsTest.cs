using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Inventory;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.MagicStrings
{
    [TestClass]
    public class ListOfConstsTest
    {
        [TestMethod]
        public void CheckListOfConsts()
        {
            //Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new ListOfConsts() }));
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new ListOfStrings() }));
        }
    }
}
