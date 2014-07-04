using System;
using System.Threading;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SharePoint.Common.Utilities
{
    public class ResourceHelper : SingletoneHelper
    {
        #region Singeton interface

        public static ResourceHelper Instance
        {
            get { return GetInstance<ResourceHelper>(); }
        }

        #endregion 

        public string GetLocalizedString(SPWeb web, string source)
        {
            if (string.IsNullOrEmpty(source) || !source.StartsWith("$Resources", StringComparison.OrdinalIgnoreCase))
                return source;
            uint language = web != null ? web.Language : (HttpContext.Current != null ? (uint)Thread.CurrentThread.CurrentUICulture.LCID : 1033);
            
            return SPUtility.GetLocalizedString(source, null, language);
        }

        public string GetLocalizedString(string source)
        {
            return GetLocalizedString(null, source);
        }
    }
}
