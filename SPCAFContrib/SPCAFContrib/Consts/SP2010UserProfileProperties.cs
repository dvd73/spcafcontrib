using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCAFContrib.Consts
{
    /// <summary>
    /// List of the SP 2010 default user profile properties according 
    /// http://technet.microsoft.com/en-us/library/hh147513.aspx
    /// </summary>
    public static class SP2010UserProfileProperties
    {
        #region classes

        public class UserProfileProperty
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string DataType { get; set; }
        }

        #endregion

        #region properties

        public static readonly List<UserProfileProperty> DefaultProperties = new List<UserProfileProperty> 
        {
            new UserProfileProperty {  Name =  "AboutMe" },
            new UserProfileProperty {  Name =  "AccountName" },
            new UserProfileProperty {  Name =  "ADGuid" },
            new UserProfileProperty {  Name =  "Assistant" },
            new UserProfileProperty {  Name =  "CellPhone" },
            new UserProfileProperty {  Name =  "Department" },
            new UserProfileProperty {  Name =  "Fax" },
            new UserProfileProperty {  Name =  "FirstName" },
            new UserProfileProperty {  Name =  "HomePhone" },
            new UserProfileProperty {  Name =  "LastName" },
            new UserProfileProperty {  Name =  "Manager" },
            new UserProfileProperty {  Name =  "Office" },
            new UserProfileProperty {  Name =  "PersonalSpace" },
            new UserProfileProperty {  Name =  "PictureURL" },
            new UserProfileProperty {  Name =  "PreferredName" },
            new UserProfileProperty {  Name =  "PublicSiteRedirect" },
            new UserProfileProperty {  Name =  "QuickLinks" },
            new UserProfileProperty {  Name =  "SID" },
            new UserProfileProperty {  Name =  "SPS-Birthday" },
            new UserProfileProperty {  Name =  "SPS-ClaimID" },
            new UserProfileProperty {  Name =  "SPS-ClaimProviderID" },
            new UserProfileProperty {  Name =  "SPS-ClaimProviderType" },
            new UserProfileProperty {  Name =  "SPS-DataSource" },
            new UserProfileProperty {  Name =  "SPS-DisplayOrder" },
            new UserProfileProperty {  Name =  "SPS-DistinguishedName" },
            new UserProfileProperty {  Name =  "SPS-DontSuggestList" },
            new UserProfileProperty {  Name =  "SPS-Dotted-line" },
            new UserProfileProperty {  Name =  "SPS-EmailOptin" },
            new UserProfileProperty {  Name =  "SPS-HireDate" },
            new UserProfileProperty {  Name =  "SPS-Interests" },
            new UserProfileProperty {  Name =  "SPS-JobTitle" },
            new UserProfileProperty {  Name =  "SPS-LastColleagueAdded" },
            new UserProfileProperty {  Name =  "SPS-LastKeywordAdded" },
            new UserProfileProperty {  Name =  "SPS-Location" },
            new UserProfileProperty {  Name =  "SPS-MemberOf" },
            new UserProfileProperty {  Name =  "SPS-MySiteUpgrade" },
            new UserProfileProperty {  Name =  "SPS-ObjectExists" },
            new UserProfileProperty {  Name =  "SPS-OWAUrl" },
            new UserProfileProperty {  Name =  "SPS-PastProjects" },
            new UserProfileProperty {  Name =  "SPS-Peers" },
            new UserProfileProperty {  Name =  "SPS-PhoneticDisplayName" },
            new UserProfileProperty {  Name =  "SPS-PhoneticFirstName" },
            new UserProfileProperty {  Name =  "SPS-PhoneticLastName" },
            new UserProfileProperty {  Name =  "SPS-ProxyAddresses" },
            new UserProfileProperty {  Name =  "SPS-ResourceSID" },
            new UserProfileProperty {  Name =  "SPS-Responsibility" },
            new UserProfileProperty {  Name =  "SPS-SavedAccountName" },
            new UserProfileProperty {  Name =  "SPS-SavedSID" },
            new UserProfileProperty {  Name =  "SPS-School" },
            new UserProfileProperty {  Name =  "SPS-SipAddress" },
            new UserProfileProperty {  Name =  "SPS-Skills" },
            new UserProfileProperty {  Name =  "SPS-SourceObjectDN" },
            new UserProfileProperty {  Name =  "SPS-StatusNotes" },
            new UserProfileProperty {  Name =  "SPS-TimeZone" },
            new UserProfileProperty {  Name =  "Title" },
            new UserProfileProperty {  Name =  "UserName" },
            new UserProfileProperty {  Name =  "UserProfile_GUID" },
            new UserProfileProperty {  Name =  "WebSite" },
            new UserProfileProperty {  Name =  "WorkEmail" },
            new UserProfileProperty {  Name =  "WorkPhone" }
        };

        #endregion
    }
}
