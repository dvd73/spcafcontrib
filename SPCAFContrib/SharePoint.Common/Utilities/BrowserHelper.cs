using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace SharePoint.Common.Utilities
{
    public class BrowserHelper
    {
        public static IEMode GetIEBrowserMode(Page page)
        {
            IEMode mode = IEMode.IE8_Standard;

            string userAgent = page.Request.UserAgent; //entire UA string
            string browser = page.Request.Browser.Type; //Browser name and Major Version #
            if (userAgent.Contains("Trident/6.0")) //IE10 has this token
            {
                if (browser == "IE7")
                {
                    mode = IEMode.IE10_CV;
                }
                else
                {
                    mode = IEMode.IE10_Standard;
                }
            }
            else if (userAgent.Contains("Trident/5.0")) //IE9 has this token
            {
                if (browser == "IE7")
                {
                    mode = IEMode.IE9_CV;
                }
                else
                {
                    mode = IEMode.IE9_Standard;
                }
            }
            else if (userAgent.Contains("Trident/4.0")) //IE8 has this token
            {
                if (browser == "IE7")
                {
                    mode = IEMode.IE8_CV;
                }
                else
                {
                    mode = IEMode.IE8_Standard;
                }
            }
            else if (!userAgent.Contains("Trident")) //Earlier versions of IE do not contain the Trident token
            {
                if (browser.Contains("MSIE 7.0"))
                    mode = IEMode.IE7;
                else
                    mode = IEMode.IE6;
            }

            return mode;
        }
    }

    public enum IEMode
    {
        IE10_Standard,
        IE10_CV,
        IE9_Standard,
        IE9_CV,
        IE8_Standard,
        IE8_CV,
        IE7,
        IE6
    }
}
