using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Sandbox
{
    [TestClass]
    public class LoadJavaScriptWithinSandboxTest
    {
        [TestMethod]
        public void CheckLoadJavaScriptWithinSandbox()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new LoadJavaScriptWithinSandbox() }));
        }
    }
}
