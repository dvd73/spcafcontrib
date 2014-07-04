using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPCAFContrib.Test.UnsafeUrlConcatinations
{
    public static class StringExtentionTmp
    {
        public static string FormatWith(this string str, string value)
        {
            return string.Format(str, value);
        }
    }

    public class TmpStore
    {
        public string Url { get; set; }
    }

    public class UnsafeUrlConcatinationsImpl
    {
        #region negative

        public void Negative_Concat_SPWeb_Url_InsideForEach()
        {
            TmpStore store = null;
            SPWeb web = null;
            var tets = Test();
            var tmpUrl = "test";

            foreach (SPWeb childWeb in web.Webs)
            {
                store.Url = tets ? web.Url + tmpUrl : web.Url;
            }
        }

        public void Negative_Concat_SPWeb_Url_InsideTrinary()
        {
            TmpStore store = null;
            SPWeb web = null;
            var tets = Test();
            var tmpUrl = "test";

            store.Url = tets ? web.Url + tmpUrl : web.Url;
        }

        public void Negative_Concat_SPWeb_Url_WithStringExtensionCall()
        {
            SPWeb web = null;
            var tets = Test();
            var tmpUrl = "test";

            var fullListUrl = web.Url + tmpUrl.FormatWith(web.SiteLogoUrl);
        }

        public void Negative_Concat_SPWeb_Url_WithStringFormatCall()
        {
            SPWeb web = null;
            var tets = Test();
            var tmpUrl = "test";

            var fullListUrl = web.Url + string.Format(tmpUrl, 1);
        }

        public void Negative_Concat_SPWeb_Url_WithObjectParams(int id)
        {
            SPWeb web = null;
            var tets = Test();

            var fullListUrl = web.Url + "/Lists/Calendar" + id;
        }

        public void Negative_Concat_SPWeb_Url_WithObject()
        {
            SPWeb web = null;
            var id = 10;
            var tets = Test();

            if (tets == true)
            {
                var fullListUrl = web.Url + "/Lists/Calendar" + id;
            }
        }

        private bool Test()
        {
            return Environment.TickCount % 10 == 0;
        }

        public void Negative_Concat_SPWeb_Url_WithMultipleObjects()
        {
            SPWeb web = null;
            var id = 10;

            var fullListUrl = web.Url + "/Lists/Calendar" + "1" + "&tmp=22" + id;
        }

        public void Negative_Concat_SPWeb_Url_WithMultipleStrings()
        {
            SPWeb web = null;
            var tmpString = "10";

            var fullListUrl = web.Url + "/Lists/Calendar" + "1" + "&tmp=22" + tmpString;
        }

        public void Negative_Concat_SPWeb_Url_WithString()
        {
            SPWeb web = null;

            var fullListUrl = web.Url + "/Lists/Calendar";
        }

        public void Negative_Concat_SPWeb_Url()
        {
            SPWeb web = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = web.Url + listUrl;
        }

        public void Negative_Concat_SPWeb_ServerRelativeUrl()
        {
            SPWeb web = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = web.ServerRelativeUrl + listUrl;
        }

        public void Negative_Concat_SPSite_Url()
        {
            Microsoft.SharePoint.SPSite site = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = site.Url + listUrl;
        }

        public void Negative_Concat_SPSite_ServerRelativeUrl()
        {
            Microsoft.SharePoint.SPSite site = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = site.ServerRelativeUrl + listUrl;
        }

        #endregion

        #region positive

        public void Positive_Concat_SPWeb_Url()
        {
            SPWeb web = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = SPUrlUtility.CombineUrl(web.Url, listUrl);
        }

        public void Positive_Concat_SPWeb_ServerRelativeUrl()
        {
            SPWeb web = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl, listUrl);
        }

        public void Positive_Concat_SPSite_Url()
        {
            Microsoft.SharePoint.SPSite site = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = SPUtility.ConcatUrls(site.Url, listUrl);
        }

        public void Positive_Concat_SPSite_ServerRelativeUrl()
        {
            Microsoft.SharePoint.SPSite site = null;

            var listUrl = "/Lists/Calendar";
            var fullListUrl = SPUtility.ConcatUrls(site.ServerRelativeUrl, listUrl);
        }

        #endregion


    }
}
