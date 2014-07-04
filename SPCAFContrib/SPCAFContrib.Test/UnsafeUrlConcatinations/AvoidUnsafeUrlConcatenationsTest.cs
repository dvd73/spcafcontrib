using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.UnsafeUrlConcatinations
{
    [TestClass]
    public class AvoidUnsafeUrlConcatenationsTest
    {
        [TestMethod]
        public void CheckAvoidUnsafeUrlConcatenations()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new AvoidUnsafeUrlConcatenations() }));
        }
    }
}
