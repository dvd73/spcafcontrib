using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Common.Utilities;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint;

namespace MOSS.Common.Utilities
{
    public class PublishingHelper : SingletoneHelper
    {
        #region Singeton interface

        public static PublishingHelper Instance
        {
            get { return GetInstance<PublishingHelper>(); }
        }

        #endregion                       

        public void AddPageLayoutsByContentType(
                       PublishingWeb publishingWeb,
                       SPContentTypeId associatedContentTypeId)
        {
            // Replace these variable values and input parameters with your own values.
            bool excludeHiddenLayouts = true;
            bool resetAllSubsitesToInherit = false;

            // Validate the input parameters.
            if (null == publishingWeb)
            {
                throw new System.ArgumentNullException("publishingWeb");
            }

            SPSite site = publishingWeb.Web.Site;
            PublishingSite publishingSite = new PublishingSite(site);

            // Retrieve a collection of all page layouts in the site collection
            // that match the content type.
            SPContentType associatedContentType = publishingSite.ContentTypes[associatedContentTypeId];
            if (null == associatedContentType)
            {
                throw new System.ArgumentException(
                    "The SPContentTypeId did not match an SPContentType in the SPSite.RootWeb",
                    "associatedContentTypeId");
            }

            PageLayout[] availablePageLayoutsByContentType = publishingWeb.GetAvailablePageLayouts(associatedContentTypeId);

            if (availablePageLayoutsByContentType.Length == 0)
            {
                PageLayout[] availablePageLayouts = publishingWeb.GetAvailablePageLayouts();
                PageLayoutCollection pageLayoutsByContentType = publishingSite.GetPageLayouts(associatedContentType, excludeHiddenLayouts);

                // Update the Web to use these page layouts when creating pages.
                publishingWeb.SetAvailablePageLayouts(availablePageLayouts.Concat(pageLayoutsByContentType.ToArray()).ToArray(), resetAllSubsitesToInherit);

                publishingWeb.Update();
            }
        }
    }
}
