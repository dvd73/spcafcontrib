using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidDollarGlobalVariableInCodeTest
    {
        [TestMethod]
        public void CheckAvoidDollarGlobalVariableInCode()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidDollarGlobalVariableInCode() }));
        }
    }
}
