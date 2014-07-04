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

namespace SPCAFContrib.Test.UnsafeUrlConcatinations.Tests
{
    [TestClass]
    public class UnsafeUrlConcatinationsTest : CodeTestBase
    {
        // unhandled
        // 1) string strItemURL = directoryList.RootFolder.ServerRelativeUrl + "/EditForm.aspx?ID=" + itemID + "";

        #region properties

        protected UnsafeUrlConcatinationsImpl Instance = new UnsafeUrlConcatinationsImpl();

        #endregion

        #region tests

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_InsideForEach()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_InsideForEach(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_InsideTrinary()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_InsideTrinary(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithStringExtensionCall()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithStringExtensionCall(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithStringFormatCall()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithStringFormatCall(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithObjectParams()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithObjectParams(1), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithObject()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithObject(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithMultipleObject()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithMultipleObjects(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithMultipleString()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithMultipleStrings(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }


        [TestMethod]
        public void Negative_Concat_SPWeb_Url_WithString()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url_WithString(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();

                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPSite_ServerRelativeUrl()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPSite_ServerRelativeUrl(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(1, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPSite_Url()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPSite_Url(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(1, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_ServerRelativeUrl()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_ServerRelativeUrl(), (assembly, type, method) =>
            {
                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Negative_Concat_SPWeb_Url()
        {
            WithTargetMethod(() => Instance.Negative_Concat_SPWeb_Url(), (assembly, type, method) =>
            {
                Assert.AreEqual(1, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Positive_Concat_SPSite_ServerRelativeUrl()
        {
            WithTargetMethod(() => Instance.Positive_Concat_SPSite_ServerRelativeUrl(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Positive_Concat_SPSite_Url()
        {
            WithTargetMethod(() => Instance.Positive_Concat_SPSite_Url(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Positive_Concat_SPWeb_ServerRelativeUrl()
        {
            WithTargetMethod(() => Instance.Positive_Concat_SPWeb_ServerRelativeUrl(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        [TestMethod]
        public void Positive_Concat_SPWeb_Url()
        {
            WithTargetMethod(() => Instance.Positive_Concat_SPWeb_Url(), (assembly, type, method) =>
            {
                Assert.AreEqual(0, method.GetUnsafeSPWebUrlConcatinations().Count());
                Assert.AreEqual(0, method.GetUnsafeSPSiteUrlConcatinations().Count());
            });
        }

        #endregion
    }
}
