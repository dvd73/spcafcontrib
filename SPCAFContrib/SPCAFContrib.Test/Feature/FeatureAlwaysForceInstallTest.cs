using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Feature
{
    [TestClass]
    public class FeatureAlwaysForceInstallTest
    {
        [TestMethod]
        public void CheckFeatureAlwaysForceInstall()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new FeatureAlwaysForceInstall() }));
        }
    }
}
