using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Publishing;
using System.Web;
using Microsoft.SharePoint.Publishing.Navigation;
using Microsoft.SharePoint;

namespace MOSS.Common.Controls
{
    public partial class MixedBreadcrumbUserControl : UserControl
    {
        protected SiteMapPath ContentMap;

        public MixedBreadcrumbUserControl()
        {
            this.Load += MixedBreadcrumbLoad;
        }

        void MixedBreadcrumbLoad(object sender, EventArgs e)
        {
            if (Page is UnsecuredLayoutsPageBase)
            {
                ContentMap.SiteMapProvider = "SPXmlContentMapProvider";
            }
            else if (Page is PublishingLayoutPage)
            {
                ContentMap.RenderCurrentNodeAsLink = false;
                
                var provider = SiteMap.Providers["CurrentNavSiteMapProviderNoEncode"] as PortalSiteMapProvider;

                if (provider != null)
                {
                    provider.IncludePages = PortalSiteMapProvider.IncludeOption.Always;
                    provider.IncludeHeadings = true;
                    provider.IncludeSubSites = PortalSiteMapProvider.IncludeOption.Always;
                    ContentMap.Provider = provider;
                }
                else
                {
                    ContentMap.SiteMapProvider = "CurrentNavSiteMapProviderNoEncode";
                }                
            }
            else
            {
                ContentMap.SiteMapProvider = "SPContentMapProvider";                
                //ContentMap.SiteMapProvider = "CurrentNavSiteMapProviderNoEncode";
                if (SPContext.Current.List == null ) ContentMap.CurrentNodeTemplate = ContentMap.RootNodeTemplate;
            }
        }
    }
}
