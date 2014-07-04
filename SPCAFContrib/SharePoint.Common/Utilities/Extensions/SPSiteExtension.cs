using System;
using System.Linq;
using Microsoft.SharePoint;
using SPDisposeCheck;
using System.Collections.Generic;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPSiteExtension
    {
        public static void UsingWebAtUrl(this SPSite site, string webServerRelativeUrl, Action<SPWeb> action)
        {
            using (var wrappedSite = new SPSite(site.Url))
            using (var web = wrappedSite.OpenWeb(webServerRelativeUrl))
                action(web);
        }

        /// <summary>
        /// <para>Use the web that is located at the specified server-relative or site-relative URL.</para> 
        /// <para>If a web is not found at the exact URL, the nearest web matching the URL will be used.</para> 
        /// </summary>
        /// <param name="site"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static void UsingWebNearestToUrl(this SPSite site, string webServerRelativeUrl, Action<SPWeb> action)
        {
            using (var wrappedSite = new SPSite(site.Url))
            using (var web = wrappedSite.OpenNearestWeb(webServerRelativeUrl))
                action(web);
        }

        /// <summary>
        /// <para>Returns the site that is located at the specified server-relative or site-relative URL.</para> 
        /// <para>If a site is not found at the exact URL, the nearest site matching the URL will be opened.</para> 
        /// </summary>
        /// <param name="site"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        [SPDisposeCheckIgnore(SPDisposeCheckID.SPDisposeCheckID_120, "This will be Disposed() elsewhere.")]
        public static SPWeb OpenNearestWeb(this SPSite site, string strUrl)
        {
            return site.OpenWeb(strUrl, false);
        }

        /// <summary>
        /// Activate a list of features
        /// </summary>
        /// <param name="site">List collection to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void ActivateFeatures(this SPSite site, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = site.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid featureId in featureIds.Except(existingFeatures))
            {
                site.Features.Add(featureId);
            }
        }

        public static void ForceActivateFeatures(this SPSite site, IEnumerable<Guid> featureIds)
        {
            foreach (Guid featureId in featureIds)
            {
                site.Features.Add(featureId, true);
            }
        }

        /// <summary>
        /// Deactivate a list of features
        /// </summary>
        /// <param name="site">List collection to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void DeactivateFeatures(this SPSite site, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = site.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid existingFeature in existingFeatures.Intersect(featureIds))
            {
                site.Features.Remove(existingFeature);
            }
        }
    }
}
