using System;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SharePoint.Common.Utilities.Extensions;

namespace SharePoint.Common.Utilities
{
    public class WebHelper : SingletoneHelper
    {
        #region Singeton interface        

        public static WebHelper Instance
        {
            get { return GetInstance<WebHelper>(); }
        }

        #endregion   

        public virtual void UsingCurrentWeb(Action<SPWeb> action)
        {
            using (var site = new SPSite(SPContext.Current.Web.Url))
            using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                action(web);
        }        

        /// <summary>
        /// <para>Use the web that is located at the specified server-relative or site-relative URL.</para> 
        /// <para>If a web is not found at the exact URL, the nearest web matching the URL will be used.</para> 
        /// </summary>
        /// <param name="site"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public virtual void UsingWebNearestToUrl(string fullUrl, Action<SPWeb> action)
        {
            var relativeUrl = UrlHelper.ConvertFullUrlToServerRelative(fullUrl);
            using (var site = new SPSite(fullUrl))
            using (var web = site.OpenNearestWeb(relativeUrl))
                action(web);
        }

        public virtual void UsingWebAtUrl(string fullUrl, Action<SPWeb> action)
        {
            var relativeUrl = UrlHelper.ConvertFullUrlToServerRelative(fullUrl);
            using (var site = new SPSite(fullUrl))
            using (var web = site.OpenWeb(relativeUrl, true))
                action(web);
        }

        public virtual void UsingWebAtUrl(string fullUrl, Action<SPSite, SPWeb> action)
        {
            var relativeUrl = UrlHelper.ConvertFullUrlToServerRelative(fullUrl);
            using (var site = new SPSite(fullUrl))
            using (var web = site.OpenWeb(relativeUrl, true))
                action(site,web);
        }

        public virtual void UsingRootWeb(Action<SPWeb> action)
        {
            // do not dispose RootWeb
            // see http://msdn.microsoft.com/en-us/library/aa973248.aspx#sharepointobjmodel__spsiteobjects
            using (var site = new SPSite(SPContext.Current.Site.Url))
            using (var web = site.OpenWeb(SPContext.Current.Site.RootWeb.ID))
                action(web);
        }

        public virtual void ProcessWebAndSubWebs(SPSite site, string siteRelativeStartUrl, Action<SPWeb> process)
        {
            var subWebUrls = new List<string>();
            using (var web = site.OpenWeb(siteRelativeStartUrl))
            {
                subWebUrls.AddRange(web.GetSubWebUrls());
                process(web);
            }
            foreach (var url in subWebUrls)
                ProcessWebAndSubWebs(site, url, process);
        }

        public virtual void ProcessWebAndSubWebs(string siteRelativeStartUrl, Action<SPWeb> process)
        {
            ProcessWebAndSubWebs(SPContext.Current.Site, siteRelativeStartUrl, process);
        }

        public string GetFileContent(string url, Encoding encoding)
        {
            string result = String.Empty;

            try
            {
                WebClient wc = new WebClient();
                wc.Credentials = CredentialCache.DefaultCredentials;
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                if (encoding != null) wc.Encoding = encoding;
                result = wc.DownloadString(url);
            }
            catch (Exception ex)
            {
                result = String.Empty;
                ex.LogError();
            }

            return result;
        }

        public string GetFileAsString(string url, Encoding encoding)
        {
            string result = String.Empty;

            try
            {
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFile file = web.GetFile(url);
                        byte[] content = file.OpenBinary();
                        Encoding enc = encoding != null ? encoding : new System.Text.ASCIIEncoding();
                        result = enc.GetString(content);
                    }
                }
            }
            catch (Exception ex)
            {
                result = String.Empty;
                ex.LogError();
            }

            return result;
        }
    }
}
