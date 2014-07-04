using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidJQueryDocumentReadyInCodeTest
    {
        [TestMethod]
        public void CheckAvoidJQueryDocumentReady()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidJQueryDocumentReadyInCode() }));
        }
    }
}
