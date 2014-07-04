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

namespace SPCAFContrib.Test.UnsafeSPListItem.Tests
{
    public static class Tmp
    {
        #region methods


        #endregion
    }

    [TestClass]
    public class UnsafeSPListItemTest : CodeTestBase
    {
        #region properties

        protected BadSPListItemServiceImpl Instance = new BadSPListItemServiceImpl();

        #endregion

        #region tests

        #region int

        [TestMethod]
        public void Negative_GetInt_ToString()
        {
            WithTargetMethod(() => Instance.Negative_GetInt_ToString(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Negative_GetInt_Cast()
        {
            WithTargetMethod(() => Instance.Negative_GetInt_Cast(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Negative_GetInt_CastNullable()
        {
            WithTargetMethod(() => Instance.Negative_GetInt_CastNullable(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetDate_ConvertToInt16()
        {
            WithTargetMethod(() => Instance.Positive_GetDate_ConvertToInt16(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetDate_ConvertToInt32()
        {
            WithTargetMethod(() => Instance.Positive_GetDate_ConvertToInt32(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetDate_ConvertToInt64()
        {
            WithTargetMethod(() => Instance.Positive_GetDate_ConvertToInt64(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetInt_As()
        {
            WithTargetMethod(() => Instance.Positive_GetInt_As(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetLong_As()
        {
            WithTargetMethod(() => Instance.Positive_GetLong_As(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        #endregion

        #region dates

        [TestMethod]
        public void Negative_GetDate_ToString()
        {
            WithTargetMethod(() => Instance.Negative_GetDate_ToString(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Negative_GetDate_Cast()
        {
            WithTargetMethod(() => Instance.Negative_GetDate_Cast(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Negative_GetDate_CastNullable()
        {
            WithTargetMethod(() => Instance.Negative_GetDate_CastNullable(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetDate_As()
        {
            WithTargetMethod(() => Instance.Positive_GetDate_As(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        [TestMethod]
        public void Positive_GetDate_ConvertToDateTime()
        {
            WithTargetMethod(() => Instance.Positive_GetDate_ConvertToDateTime(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(0, unsafeCasts.Count());
            });
        }

        #endregion

        #region user value

        //

        [TestMethod]
        public void Negative_GetSPFieldUserValue_Cast()
        {
            WithTargetMethod(() => Instance.Negative_GetSPFieldUserValue_Cast(null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                var unsafeCasts = method.GetUnsafeSPListItemCastInstructions();
                Assert.AreEqual(1, unsafeCasts.Count());
            });
        }

        #endregion

        #endregion
    }
}
