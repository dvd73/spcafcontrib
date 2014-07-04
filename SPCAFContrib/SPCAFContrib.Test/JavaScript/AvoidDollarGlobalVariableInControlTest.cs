using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Control;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidDollarGlobalVariableInControlTest
    {
        [TestMethod]
        public void CheckAvoidDollarGlobalVariableInControl()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidDollarGlobalVariableInControl() }));
        }
    }
}
