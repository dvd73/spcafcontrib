using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Test.SearchString.Tests
{
    [TestClass]
    public class SearchStringTest : CodeTestBase
    {
        #region properties

        protected TestStringClassImpl Instance = new TestStringClassImpl();

        #endregion

        #region tests

        [TestMethod]
        public void CanFindAllStringConsts()
        {
            WithTargetType(Instance.GetType().Name, (assembly, type) =>
            {
                var hitCount = 0;

                type.SearchMethodStrings((method, instruction) => { hitCount++; });
                type.SearchConstStrings(constField => { hitCount++; });

                Assert.AreEqual(6, hitCount);
            });
        }

        #endregion
    }
}
