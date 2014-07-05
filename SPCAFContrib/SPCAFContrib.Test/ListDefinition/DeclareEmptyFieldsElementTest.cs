using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.ListDefinition
{
    [TestClass]
    public class DeclareEmptyFieldsElementTest
    {
        [TestMethod]
        public void CheckDeclareEmptyFieldsElement()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DeclareEmptyFieldsElement() }));
        }
    }
}
