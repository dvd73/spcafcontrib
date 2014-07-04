using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.ListDefinition
{
    [TestClass]
    public class DoNotAllowDeletionForListTest
    {
        [TestMethod]
        public void CheckFieldIdShouldBeUppercase()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotAllowDeletionForList() }));
        }
    }
}
