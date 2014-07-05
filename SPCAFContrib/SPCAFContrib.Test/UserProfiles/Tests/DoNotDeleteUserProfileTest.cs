using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCAFContrib.Rules.Code;
using SPCAFContrib.Test.Common;

namespace SPCAFContrib.Test.UserProfiles.Tests
{
    [TestClass]
    public class DoNotDeleteUserProfileTest
    {
        [TestMethod]
        public void CheckDoNotDeleteUserProfile()
        {
            Assert.AreEqual(true, SPCAFTestHelper.ExecuteTest(new[] { new DoNotDeleteUserProfile() }));
        }
    }
}
