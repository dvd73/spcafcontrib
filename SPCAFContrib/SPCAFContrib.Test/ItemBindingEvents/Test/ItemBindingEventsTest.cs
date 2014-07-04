using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Test.Common;
using SPCAFContrib.Test.Extensions;
using SPCAFContrib.Extensions;
using System.Reflection;
using SPCAFContrib.Rules.Code;
using System.Diagnostics;

namespace SPCAFContrib.Test.ItemBindingEvents.Test
{
    public static class Tmp
    {
        #region methods



        #endregion
    }

    [TestClass]
    public class ItemBindingEventsTest : CodeTestBase
    {
        #region properties

        protected BadItemBindingImpl Instance = new BadItemBindingImpl();

        #endregion

        #region tests

        #region data list

        [TestMethod]
        public void Positive_ConstructorAssignedDataListView_ItemCreated()
        {
            WithTargetMethod(() => Instance.Positive_ConstructorAssignedDataListView_ItemCreated(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsDataListItemEventHandler());
            });
        }

        public void Positive_ConstructorAssignedDataListView_ItemDataBound()
        {
            WithTargetMethod(() => Instance.Positive_ConstructorAssignedDataListView_ItemDataBound(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsDataListItemEventHandler());
            });
        }

        public void Positive_IsPostBackAssignedDataListView_ItemCreated()
        {
            WithTargetMethod(() => Instance.Positive_IsPostBackAssignedDataListView_ItemCreated(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsDataListItemEventHandler());
            });
        }

        public void Positive_IsPostBackAssignedDataListView_ItemDataBound()
        {
            WithTargetMethod(() => Instance.Positive_IsPostBackAssignedDataListView_ItemDataBound(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsDataListItemEventHandler());
            });
        }

        #endregion

        #region repeater

        [TestMethod]
        public void Positive_ConstructorAssignedView_ItemCreated()
        {
            WithTargetMethod(() => Instance.Positive_ConstructorAssignedView_ItemCreated(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsRepeaterItemEventHandler());
            });
        }

        [TestMethod]
        public void Positive_ConstructorAssignedView_ItemDataBound()
        {
            WithTargetMethod(() => Instance.Positive_ConstructorAssignedView_ItemDataBound(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsRepeaterItemEventHandler());
            });
        }

        [TestMethod]
        public void Positive_IsPostBackAssignedView_ItemCreated()
        {
            WithTargetMethod(() => Instance.Positive_IsPostBackAssignedView_ItemCreated(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsRepeaterItemEventHandler());
            });
        }

        [TestMethod]
        public void Positive_IsPostBackAssignedView_ItemDataBound()
        {
            WithTargetMethod(() => Instance.Positive_IsPostBackAssignedView_ItemDataBound(null, null), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
                Assert.AreEqual(true, method.IsRepeaterItemEventHandler());
            });
        }

        #endregion

        #endregion
    }
}
