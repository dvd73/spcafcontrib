using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Publishing
{
    [TestClass]
    public class GetPublishingPagesNoParamsTest
    {
        #region tests

        [TestMethod]
        public void GetPublishingPagesNoParams()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new GetPublishingPagesNoParams()}));
        }

        #endregion
    }
}
