using System;
using Microsoft.SharePoint;
using System.Security.Principal;
using SharePoint.Common.Utilities.Extensions;
using System.Web;

namespace SharePoint.Common.Utilities
{
    public class UserHelper : SingletoneHelper
    {
        #region Singeton interface

        public static UserHelper Instance
        {
            get { return GetInstance<UserHelper>(); }
        }

        #endregion   

        public SPUser CurrentUser
        {
            get {
                SPUser result = null;
                SPContext currentContext;
                try
                {
                    //Getting the current context           
                    currentContext = SPContext.Current;
                }
                catch (InvalidOperationException)
                {
                    currentContext = null;
                }
                if (currentContext != null && currentContext.Web.CurrentUser != null)
                {
                    result = SPContext.Current.Web.CurrentUser;
                }
                else if (System.Web.HttpContext.Current != null)
                {
                    string uName = System.Web.HttpContext.Current.User.Identity.Name;
                }

                return result;
            }
        }

        public string CurrentUserLoginName
        {
            get
            {
                string result = String.Empty;
                SPUser user = CurrentUser;

                if (user != null)
                {
                    result = user.LoginName;
                    if (String.Equals(result, @"sharepoint\system", StringComparison.CurrentCultureIgnoreCase) && 
                        HttpContext.Current != null && HttpContext.Current.User != null)
                    {
                        result = HttpContext.Current.User.Identity.Name;
                    }
                }

                return result;
            }
        }
        /// <summary>
        /// When I needed to get SPUser-object I always utilized SPWeb.EnsureUser method. EnsureUser looks for the specified user login inside SPWeb.SiteUsers collection, 
        /// and if the login isn’t found, turns to ActiveDirectory for the purpose of retrieving the user information from there. If such information is found, 
        /// it will be added to SPWeb.SiteUsers and for the next time it will be returned directly from SPWeb.SiteUsers.
        /// So, we have the fact that when EnsureUser is called, SPWeb-object potentially can be changed (changes affect SPWeb.SiteUsers collection, to be more precise). 
        /// Each time we change SPWeb-object, it’s highly recommended to make sure that SPWeb.AllowUnsafeUpdates = true. It’s especially topical, when EnsureUser is called 
        /// from Page during GET-request (Page.IsPostback = false). Otherwise, “Updates are currently disallowed on GET requests. 
        /// To allow updates on a GET, set the ‘AllowUnsafeUpdates’ property on SPWeb” is thrown.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public SPUser SafeEnsureUser(SPWeb web, string loginName)
        {
            SPUser res = null;
            if (!web.AllowUnsafeUpdates)
            {
                bool oldAllowUnsafeUpdate = web.AllowUnsafeUpdates;
                try
                {
                    web.AllowUnsafeUpdates = true;
                    res = web.EnsureUser(loginName);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
                finally
                {
                    web.AllowUnsafeUpdates = oldAllowUnsafeUpdate;
                }
            }
            else
                try
                {
                    res = web.EnsureUser(loginName);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }

            return res;
        }
    }
}
