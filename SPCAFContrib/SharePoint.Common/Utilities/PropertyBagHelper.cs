using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SharePoint.Common.Utilities
{
    public class PropertyBagHelper : SingletoneHelper
    {
        #region Singeton interface

        public static PropertyBagHelper Instance
        {
            get { return GetInstance<PropertyBagHelper>(); }
        }

        #endregion    

        public virtual string GetStringValue(SPWeb web, string key)
        {
            SPSite site = null;
            if (SPContext.Current != null)
            {
                site = SPContext.Current.Site;
                web = web == null ? SPContext.Current.Site.RootWeb : web;
            }
            else
            {
                if (web != null)
                {
                    site = web.Site;
                }
            }

            object siteCollectionSetting = web.AllProperties[key];

            return siteCollectionSetting != null ? siteCollectionSetting.ToString() : String.Empty;
        }
    }
}
