using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Extensions;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.TryCatch.Tests
{
    [TestClass]
    public class TryCatchTest : CodeTestBase
    {
        #region properties

        protected BadTryCatchImpl Instance = new BadTryCatchImpl();

        #endregion

        #region tests

        #region sharepoint

        [TestMethod]
        public void Neutral_Using_Static_StreamWriter_WithoutTryCatch()
        {
            WithTargetMethod(() => BadTryCatchImpl.Neutral_Using_Static_StreamWriter_WithoutTryCatch(""), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_StreamWriter_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_StreamWriter_WithoutTryCatch(""), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_Auto_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_Auto_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_Auto_External_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_Auto_External_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_WithMethod_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_WithMethod_WithoutTryCatch(string.Empty), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_Manual_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Negative_Using_Manual_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(true, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_In_Using_In_Using_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_In_Using_In_Using_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_In_Double_Using_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_In_Double_Using_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_In_Using_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_In_Using_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_Custom_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_Custom_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Using_Double_Custom_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Using_Double_Custom_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }


        [TestMethod]
        public void Neutral_While_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_While_WithoutTryCatch(null), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void LoadCustomersData()
        {
            WithTargetMethod(() => Instance.LoadCustomersData(null), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        #endregion

        #region negative

        [TestMethod]
        public void Negative_TryBody_CatchBody_FirstCallNotWrapped()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_CatchBody_FirstCallNotWrapped(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_TryBody_CatchBody_FirstEnumeratorNotWrapped()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_CatchBody_FirstEnumeratorNotWrapped(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_TryBody_CatchBody_FirstAndLastCallNotWrapped()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_CatchBody_FirstAndLastCallNotWrapped(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_TryBody_CatchBody_LastCallNotWrapped()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_CatchBody_LastCallNotWrapped(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_TryBody_CatchEmpty()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_CatchEmpty(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(true, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_TryBody_CatchEmptyWithExceptionHandling()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_CatchEmptyWithExceptionHandling(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(true, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_TryBody_NoCatch_FinallyEmpty()
        {
            WithTargetMethod(() => Instance.Negative_TryBody_NoCatch_FinallyEmpty(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(true, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        #endregion

        #region neutral

        [TestMethod]
        public void Neutral_Foreach_WithoutTryCatch()
        {
            WithTargetMethod(() => Instance.Neutral_Foreach_WithoutTryCatch(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_EmptyMethod()
        {
            WithTargetMethod(() => Instance.Neutral_EmptyMethod(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(false, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Neutral_Empty_ZeroMethodCall()
        {
            WithTargetMethod(() => Instance.Neutral_Empty_ZeroMethodCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.InsideMethodsCallsCount());
            });
        }

        [TestMethod]
        public void Neutral_Empty_OneMethodCall()
        {
            WithTargetMethod(() => Instance.Neutral_Empty_OneMethodCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(1, method.InsideMethodsCallsCount());
            });
        }

        [TestMethod]
        public void Neutral_Empty_TwoMethodCall()
        {
            WithTargetMethod(() => Instance.Neutral_Empty_TwoMethodCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(2, method.InsideMethodsCallsCount());
            });
        }

        [TestMethod]
        public void Neutral_Empty_ThreeMethodCall()
        {
            WithTargetMethod(() => Instance.Neutral_Empty_ThreeMethodCall(), (assembly, type, method) =>
            {
                Assert.AreEqual(3, method.InsideMethodsCallsCount());
            });
        }

        #endregion

        #region positive

        [TestMethod]
        public void Positive_FirstLevel_TryCatch_Helper()
        {
            WithTargetMethod(() => Instance.Positive_FirstLevel_TryCatch_Helper(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(1, method.InsideMethodsCallsCount());

                var firstInsideCallMethod = method.GetFirstOrDefaultInsideMethod();

                Assert.IsNotNull(firstInsideCallMethod);

                Assert.AreEqual(false, firstInsideCallMethod.HasEmptyExceptionHandler());
                Assert.AreEqual(true, firstInsideCallMethod.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Positive_FirstLevel_TryCatch_Extension_Helper()
        {
            WithTargetMethod(() => Instance.Positive_FirstLevel_TryCatch_Extension_Helper(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(1, method.InsideMethodsCallsCount());

                var firstInsideCallMethod = method.GetFirstOrDefaultInsideMethod();

                Assert.IsNotNull(firstInsideCallMethod);

                Assert.AreEqual(false, firstInsideCallMethod.HasEmptyExceptionHandler());
                Assert.AreEqual(true, firstInsideCallMethod.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_FirstLevel_TryCatch_Helper()
        {
            WithTargetMethod(() => Instance.Negative_FirstLevel_TryCatch_Helper(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(1, method.InsideMethodsCallsCount());

                var firstInsideCallMethod = method.GetFirstOrDefaultInsideMethod();

                Assert.IsNotNull(firstInsideCallMethod);

                Assert.AreEqual(true, firstInsideCallMethod.HasEmptyExceptionHandler());
                Assert.AreEqual(true, firstInsideCallMethod.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Negative_FirstLevel_TryCatch_Extension_Helper()
        {
            WithTargetMethod(() => Instance.Negative_FirstLevel_TryCatch_Extension_Helper(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasExceptionHandlers());
                Assert.AreEqual(1, method.InsideMethodsCallsCount());

                var firstInsideCallMethod = method.GetFirstOrDefaultInsideMethod();

                Assert.IsNotNull(firstInsideCallMethod);

                Assert.AreEqual(true, firstInsideCallMethod.HasEmptyExceptionHandler());
                Assert.AreEqual(true, firstInsideCallMethod.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Positive_TryBody_CatchBody()
        {
            WithTargetMethod(() => Instance.Positive_TryBody_CatchBody(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Positive_TryBody_CatchBody_FullyWrappedMethod()
        {
            WithTargetMethod(() => Instance.Positive_TryBody_CatchBody_FullyWrappedMethod(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Positive_TryBody_CatchBodyWithThrow()
        {
            WithTargetMethod(() => Instance.Positive_TryBody_CatchBodyWithThrow(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        [TestMethod]
        public void Positive_TryBody_CatchBodyWithThrowNewExcption()
        {
            WithTargetMethod(() => Instance.Positive_TryBody_CatchBodyWithThrowNewExcption(), (assembly, type, method) =>
            {
                Assert.AreEqual(true, method.HasExceptionHandlers());
                Assert.AreEqual(false, method.HasEmptyExceptionHandler());
                Assert.AreEqual(true, method.HasFullTryCatchCover());
            });
        }

        #endregion

        #endregion
    }
}
