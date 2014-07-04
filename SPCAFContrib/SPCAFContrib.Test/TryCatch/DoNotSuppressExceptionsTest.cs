using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.TryCatch
{

    [TestClass]
    public class DoNotSuppressExceptionsTest
    {
        [TestMethod]
        public void CheckDoNotSuppressExceptions()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotSuppressExceptions() }));
        }
    }
}
