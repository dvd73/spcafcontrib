using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Xml;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.FieldDefinition
{
     [TestClass]
    public class FieldIdShouldBeUppercaseTest 
    {
         [TestMethod]
         public void CheckFieldIdShouldBeUppercase()
         {
             Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new []{new FieldIdShouldBeUppercase()}));
         }
    }
}
