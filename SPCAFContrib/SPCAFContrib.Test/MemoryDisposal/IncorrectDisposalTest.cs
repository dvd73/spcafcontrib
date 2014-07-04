using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Experimental.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.MemoryDisposal
{
    [TestClass]
    public class IncorrectDisposalTest
    {
        [TestMethod]
        public void CheckIncorrectDisposal()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new IncorrectDisposal() }));
        }
    }
}
