using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Common;
using Mono.Cecil.Cil;
using SPCAFContrib.Test.Extensions;
using SPCAFContrib.Test.UnsafeTaxonomy;

namespace SPCAFContrib.Test.UnsafeTaxonomy.Tests
{
    [TestClass]
    public class UnsafeTaxonomyTest : CodeTestBase
    {
        #region properties

        protected UnsafeTaxonomyImpl Instance = new UnsafeTaxonomyImpl();

        #endregion

        #region tests

        #region groups

        [TestMethod]
        public void Negative_GroupIndex_String()
        {
            WithTargetMethod(() => Instance.Negative_GroupIndex_String(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeTaxonomyGroupStringIndexCall().Count());
                Assert.AreEqual(0, method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall().Count());
            });
        }

        [TestMethod]
        public void Negative_TermSetCollectionIndex_String()
        {
            WithTargetMethod(() => Instance.Negative_TermSetCollectionIndex_String(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeTaxonomyGroupStringIndexCall().Count());
                Assert.AreEqual(1, method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall().Count());
            });
        }

        [TestMethod]
        public void Positive_GroupIndex_Int()
        {
            WithTargetMethod(() => Instance.Positive_GroupIndex_Int(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeTaxonomyGroupStringIndexCall().Count());
                Assert.AreEqual(0, method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall().Count());
            });
        }

        [TestMethod]
        public void Positive_TermSetCollectionIndex_Guid()
        {
            WithTargetMethod(() => Instance.Positive_TermSetCollectionIndex_Guid(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeTaxonomyGroupStringIndexCall().Count());
                Assert.AreEqual(0, method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall().Count());
            });
        }

        [TestMethod]
        public void Positive_TermSetCollectionIndex_Int()
        {
            WithTargetMethod(() => Instance.Positive_TermSetCollectionIndex_Int(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeTaxonomyGroupStringIndexCall().Count());
                Assert.AreEqual(0, method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall().Count());
            });
        }

        #endregion

        #endregion
    }
}
