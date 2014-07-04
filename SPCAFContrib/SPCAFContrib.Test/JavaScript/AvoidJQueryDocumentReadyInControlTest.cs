using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Control;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.JavaScript
{
    [TestClass]
    public class AvoidJQueryDocumentReadyInControlTest
    {
        [TestMethod]
        public void CheckAvoidJQueryDocumentReadyInControl()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidJQueryDocumentReadyInControl() }));
        }
    }
}
