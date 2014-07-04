using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Server.Search.Query;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;

namespace SPCAFContrib.Test.UserProfiles
{

    public class AvoidEnumeratingAllUserProfilesImpl
    {
        public void AddSearchProperty(Microsoft.SharePoint.SPSite site)
        {
            var searchQuery = new KeywordQuery(site);

            searchQuery.SelectProperties.Add("testSearchProp-1");
            searchQuery.SelectProperties.Add("testSearchProp-2");
        }

        public void SetUserProfileProperty(UserProfile profile)
        {
            profile["setTestProperty"].Value = 1;
        }

        public object GetUserProfileProperty(UserProfile profile)
        {
            return profile["getTestProperty"].Value;
        }

        public void EnumeratingAllUserProfiles()
        {
            Microsoft.SharePoint.SPSite site = new Microsoft.SharePoint.SPSite("http://localhost");
            using (var tmpSite = new Microsoft.SharePoint.SPSite(site.ID))
            {
                var serviceContext = SPServiceContext.GetContext(tmpSite.WebApplication.ServiceApplicationProxyGroup, SPSiteSubscriptionIdentifier.Default);
                var profileManager = new UserProfileManager(serviceContext);

                foreach (UserProfile up in profileManager)
                {
                }

                //IEnumerator profiles = profileManager.GetEnumerator();
                //long counter = 0;
                //while (profiles.MoveNext())
                //    counter++;
            }
        }

        public void GetUserProfilePage()
        {
            Microsoft.SharePoint.SPSite site = new Microsoft.SharePoint.SPSite("http://localhost");
            using (var tmpSite = new Microsoft.SharePoint.SPSite(site.ID))
            {
                var serviceContext = SPServiceContext.GetContext(tmpSite.WebApplication.ServiceApplicationProxyGroup, SPSiteSubscriptionIdentifier.Default);
                var profileManager = new UserProfileManager(serviceContext);

                // get specific pages
                IEnumerator t = profileManager.GetEnumerator(1, 10);
            }
        }
    }
}
