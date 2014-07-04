using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil.Cil;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Extensions;

namespace SPCAFContrib.Test.UnsafeSPListCollectionCalls.Tests
{
    [TestClass]
    public class UnsafeSPListCollectionCallTest : CodeTestBase
    {
        #region properties

        protected UnsafeSPListCollectionCall Instance = new UnsafeSPListCollectionCall();

        #endregion

        #region tests

        #region negative

        [TestMethod]
        public void Negative_EnumeratorCall()
        {
            WithTargetMethod(() => Instance.Negative_EnumeratorCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(1, method.GetSPListCollectionEnumeratorCalls().Count());
                Assert.AreEqual(0, method.GetSPListCollectionStringIndexCall().Count());

                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqCastCall().Count());
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqOfTypeCall().Count());
            });
        }

        [TestMethod]
        public void Negative_EnumeratorLinqCastCall()
        {
            WithTargetMethod(() => Instance.Negative_EnumeratorLinqCastCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorCalls().Count());
                Assert.AreEqual(0, method.GetSPListCollectionStringIndexCall().Count());

                Assert.AreEqual(1, method.GetSPListCollectionEnumeratorLinqCastCall().Count());
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqOfTypeCall().Count());
            });
        }

        [TestMethod]
        public void Negative_EnumeratorLinqOfTypeCall()
        {
            WithTargetMethod(() => Instance.Negative_EnumeratorLinqOfTypeCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorCalls().Count());
                Assert.AreEqual(0, method.GetSPListCollectionStringIndexCall().Count());

                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqCastCall().Count());
                Assert.AreEqual(1, method.GetSPListCollectionEnumeratorLinqOfTypeCall().Count());
            });
        }

        [TestMethod]
        public void Negative_StringIndexCall()
        {
            WithTargetMethod(() => Instance.Negative_StringIndexCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorCalls().Count());
                Assert.AreEqual(1, method.GetSPListCollectionStringIndexCall().Count());

                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqCastCall().Count());
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqOfTypeCall().Count());
            });
        }



        #endregion

        #region positive

        [TestMethod]
        public void Positive_GuidIndexCall()
        {
            WithTargetMethod(() => Instance.Positive_GuidIndexCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorCalls().Count());
                Assert.AreEqual(0, method.GetSPListCollectionStringIndexCall().Count());

                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqCastCall().Count());
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqOfTypeCall().Count());
            });
        }

        [TestMethod]
        public void Positive_IntIndexCall()
        {
            WithTargetMethod(() => Instance.Positive_IntIndexCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorCalls().Count());
                Assert.AreEqual(0, method.GetSPListCollectionStringIndexCall().Count());

                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqCastCall().Count());
                Assert.AreEqual(0, method.GetSPListCollectionEnumeratorLinqOfTypeCall().Count());
            });
        }

        #endregion

        #endregion
    }
}
