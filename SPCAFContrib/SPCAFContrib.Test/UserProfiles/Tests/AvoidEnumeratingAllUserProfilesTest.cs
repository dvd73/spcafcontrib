using SPCAFContrib.Test.Common;
using SPCAFContrib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SPCAFContrib.Test.Extensions;

namespace SPCAFContrib.Test.UserProfiles.Tests
{
    [TestClass]
    public class AvoidEnumeratingAllUserProfilesTest : CodeTestBase
    {
        #region properties

        protected AvoidEnumeratingAllUserProfilesImpl Instance = new AvoidEnumeratingAllUserProfilesImpl();

        #endregion

        #region tests

        [TestMethod]
        public void Negative_Tmp()
        {
            WithTargetMethod(() => Instance.EnumeratingAllUserProfiles(), (assembly, type, method) =>
            {
                type.TraceMethodInstructions();
            });
         }

        [TestMethod]
        public void Negative_MissingUserProfileEnumeration()
        {
            WithTargetMethod(() => Instance.EnumeratingAllUserProfiles(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasUserProfileEnumeration());
            });
        }

        [TestMethod]
        public void Positive_MissingUserProfileEnumeration()
        {
            WithTargetMethod(() => Instance.GetUserProfilePage(), (assembly, type, method) =>
            {
                Assert.AreEqual(false, method.HasUserProfileEnumeration());
            });
        }
        #endregion
    }
}
