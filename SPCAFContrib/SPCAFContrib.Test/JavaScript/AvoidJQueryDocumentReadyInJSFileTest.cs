using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.JavaScript;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidJQueryDocumentReadyInJSFileTest
    {
        [TestMethod]
        public void CheckAvoidJQueryDocumentReadyInJSFile()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidJQueryDocumentReadyInJSFile() }));
        }
    }
}
