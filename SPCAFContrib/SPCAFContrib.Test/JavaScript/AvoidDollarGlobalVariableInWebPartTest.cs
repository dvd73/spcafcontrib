using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidDollarGlobalVariableInWebPartTest
    {
        [TestMethod]
        public void CheckAvoidDollarGlobalVariableInWebPart()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidDollarGlobalVariableInWebPart() }));
        }
    }
}
