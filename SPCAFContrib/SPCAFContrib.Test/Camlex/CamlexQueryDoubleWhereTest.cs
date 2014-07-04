using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.Camlex
{
    [TestClass]
    public class CamlexQueryDoubleWhereTest
    {
        [TestMethod]
        public void CheckCamlexQueryDoubleWhere()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new CamlexQueryDoubleWhere() }));
        }
    }
}
