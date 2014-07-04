using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.ListItemModification
{
    [TestClass]
    public class ListModificationFromJobTest
    {
        [TestMethod]
        public void ListModificationFromJob()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new ListModificationFromJob()}));
        }
    }
}
