using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Test.Extensions;

namespace SPCAFContrib.Test.UnsafeStringComporation.Tests
{
    [TestClass]
    public class UnsafeStringComporationTest : CodeTestBase
    {
        #region properties

        protected UnsafeStringComporationImpl Instance = new UnsafeStringComporationImpl();

        #endregion

        [TestMethod]
        public void Negative_Linq_Expression()
        {
            WithTargetMethod(() => Instance.Negative_Linq_Expression(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPObjectStringComparison().Count());
            });
        }

        [TestMethod]
        public void Negative_GroupIndex_String()
        {
            WithTargetMethod(() => Instance.Negative_SPContentType_Name(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPObjectStringComparison().Count());
            });
        }
    }
}
