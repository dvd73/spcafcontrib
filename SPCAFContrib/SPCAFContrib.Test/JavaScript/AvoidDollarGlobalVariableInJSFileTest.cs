using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.JavaScript;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidDollarGlobalVariableInJSFileTest
    {
        [TestMethod]
        public void CheckAvoidDollarGlobalVariableInJSFile()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidDollarGlobalVariableInJSFile() }));
        }
    }
}
