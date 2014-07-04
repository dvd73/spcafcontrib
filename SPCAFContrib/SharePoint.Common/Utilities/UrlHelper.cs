using System;
using System.Linq;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities
{
    public static class UrlHelper
    {
        /// <summary>
        ///  Uses given web to figure out relative urls. Eg. ~site on site http://moss-dev returns /.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SharePointUrlToRelativeUrl(this SPWeb web, string url)
        {
            url = SharePointUrlToAbsoluteUrl(web, url);
            return (new System.Uri(url)).PathAndQuery;
        }

        /// <summary>
        ///  Uses current context to figure out relative urls. Eg. ~site on site http://moss-dev returns /.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SharePointUrlToRelativeUrl(string url)
        {
            return SharePointUrlToRelativeUrl(null, url);
        }

        public static string SharePointUrlToAbsoluteUrl(this SPWeb web, string url)
        {
            url = url.ToLower().Trim();
            if (url.StartsWith("~sitecollection", StringComparison.CurrentCultureIgnoreCase) || url.StartsWith("/") || url.StartsWith("~site", StringComparison.CurrentCultureIgnoreCase))
            {
                string siteCollectionUrl, webUrl;
                if (web == null)
                {
                    siteCollectionUrl = SPContext.Current.Site.Url;
                    webUrl = SPContext.Current.Web.Url;
                }
                else
                {
                    siteCollectionUrl = web.Site.Url;
                    webUrl = web.Url;
                }
                url = url.Replace("~sitecollection", siteCollectionUrl);
                url = url.Replace("~site", webUrl);
                if (url.StartsWith("/"))
                {
                    siteCollectionUrl = siteCollectionUrl.TrimEnd('/');
                    url = siteCollectionUrl + url;
                }
            }
            return url;
        }

        /// <summary>
        ///  Uses current context to figure out relative urls. Eg. ~site on site http://moss-dev returns http://moss-dev.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SharePointUrlToAbsoluteUrl(string url)
        {
            return SharePointUrlToAbsoluteUrl(null, url);
        }

        /// <summary>
        /// Compares the two URIs, 
        /// "http://Contoso.com/folder/file" and "http://www.Contoso.com/otherfolder/otherfile"
        /// will return true
        /// </summary>
        /// <param name="firstUri"></param>
        /// <param name="secondUri"></param>
        /// <returns></returns>
        public static bool AreInSameBaseDomain(Uri firstUri, Uri secondUri)
        {
            var firstBaseAuthority = GetBaseAuthority(firstUri);
            var secondBaseAuthority = GetBaseAuthority(secondUri);

            return firstBaseAuthority.Equals(secondBaseAuthority, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the top- and second-level domains of an Uri. If domain is one-part, return Authority
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetBaseAuthority(Uri uri)
        {
            var authorityLastPart = uri.Authority.Split(new char[] { '.' }).Reverse().Take(2).Reverse().ToList();
            if (authorityLastPart.Count == 2)
                return string.Format("{0}.{1}", authorityLastPart[0], authorityLastPart[1]);
            else
                return uri.Authority;
        }

        /// <summary>
        /// <para>Examples:</para>
        /// <para>http://localhost/Pages/default.aspx --> /Pages/default.aspx</para>
        /// <para>http://localhost/Pages/default.aspx?productId=45 --> /Pages/default.aspx?productId=45</para>
        /// <para>http://localhost/fi/Sivut/default.aspx?productId=45 --> /fi/Sivut/default.aspx?productId=45</para>
        /// </summary>
        /// <param name="fullUrl"></param>
        /// <returns></returns>
        public static string ConvertFullUrlToServerRelative(string fullUrl)
        {
            return new Uri(fullUrl).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}