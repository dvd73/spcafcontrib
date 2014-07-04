using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SharePoint.Common.Utilities;

namespace MOSS.Common.Utilities
{
    public class MOSSPropertyBagHelper : PropertyBagHelper
    {
        public new static MOSSPropertyBagHelper Instance
        {
            get { return GetInstance<MOSSPropertyBagHelper>(); }
        }   

        public override string GetStringValue(SPWeb web, string key)
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

            object serverSetting = SPServer.Local.Properties[key];
            object webApplicationSetting = site.WebApplication.Properties[key];
            object siteCollectionSetting = null;
            if (web != null)
                siteCollectionSetting = web.Properties[key];

            return serverSetting != null ? serverSetting.ToString() :
                webApplicationSetting != null ? webApplicationSetting.ToString() :
                siteCollectionSetting != null ? siteCollectionSetting.ToString() : String.Empty;
        }

        public void DeleteStringValue(Guid siteId, string key)
        {
            using (SPSite elevatedSite = SecurityHelper.GetElevatedSite(siteId))
            {
                using (SPWeb rootWeb = elevatedSite.RootWeb)
                {
                    if (rootWeb.Properties.ContainsKey(key))
                    {
                        rootWeb.Properties[key] = null;
                        rootWeb.Properties.Update();
                    }
                }
            }
        }

        public virtual void SetStringValue(Guid siteId, string key, string value)
        {
            using (SPSite site = !siteId.Equals(Guid.Empty) ? SecurityHelper.GetElevatedSite(siteId) : SecurityHelper.GetCurrentSiteAsElevated())
            {
                using (SPWeb rootWeb = site.RootWeb)
                {
                    rootWeb.AllowUnsafeUpdates = true;

                    if (!rootWeb.Properties.ContainsKey(key))
                    {
                        rootWeb.Properties.Add(key, value);
                    }
                    else
                    {
                        rootWeb.Properties[key] = value;
                    }
                    rootWeb.Properties.Update();

                    rootWeb.AllowUnsafeUpdates = false;
                }
            }
        }
    }
}
