using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.SPSite
{
    [TestClass]
    public class SpecifySPZoneInSPSiteTest
    {
        [TestMethod]
        public void UseSpecifySPZoneInSPSite()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new SpecifySPZoneInSPSite() }));
        }
    }
}
