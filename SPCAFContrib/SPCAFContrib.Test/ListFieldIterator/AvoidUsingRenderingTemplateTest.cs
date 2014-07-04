using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Control;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.ListInstance
{
    [TestClass]
    public class AvoidUsingRenderingTemplateTest
    {
        [TestMethod]
        public void CheckAvoidUsingRenderingTemplate()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidUsingRenderingTemplate() }));
        }
    }
}
