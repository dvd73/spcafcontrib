using Microsoft.SharePoint;
using System;

namespace MOSS.Common.Utilities
{
    public class SecurityHelper
    {
        public static SPSite GetCurrentSiteAsElevated()
        {
            return GetElevatedSite(SPContext.Current.Site.ID);
        }

        public static SPSite GetElevatedSite(SPSite theSite)
        {
            return new SPSite(theSite.ID, theSite.Zone, GetSystemToken(theSite));
        }

        public static SPSite GetElevatedSite(Guid id)
        {
            using (SPSite theSite = new SPSite(id))
            {
                if (SPContext.Current != null)
                    return new SPSite(id, SPContext.Current.Site.Zone, GetSystemToken(theSite));
                else
                    return new SPSite(id, GetSystemToken(theSite));
            }
        }

        public static SPUserToken GetSystemToken(SPSite site)
        {
            SPUserToken res = null;
            bool oldCatchAccessDeniedException = site.CatchAccessDeniedException;
            try
            {
                site.CatchAccessDeniedException = false;
                res = site.SystemAccount.UserToken;
            }
            catch (UnauthorizedAccessException)
            {                
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite elevatedSPSite = new SPSite(site.ID))
                    {
                        res = elevatedSPSite.SystemAccount.UserToken; 
                    }
                });                
            }
            finally
            {
                site.CatchAccessDeniedException = oldCatchAccessDeniedException;
            }
            return res;
        }
    }
}
