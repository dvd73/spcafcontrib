using System;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.WebPartPages;
using WebPart = Microsoft.SharePoint.WebPartPages.WebPart;

namespace MOSS.Common.Pages
{
    public class BasePageLayout : PublishingLayoutPage
    {
        protected SPWebPartManager WPManager;
        protected SPWeb web;

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            WPManager = SPWebPartManager.GetCurrentWebPartManager(this) as SPWebPartManager;
            web = SPContext.Current != null ? SPContext.Current.Web : null;

            PageLoadHandler(sender, e);
        }

        #endregion

        #region Implementation details

        protected virtual void PageLoadHandler(object sender, EventArgs e)
        {
            if (this.Master != null)
            {
                if (WPManager != null && WPManager.DisplayMode.Name.Equals(WebPartManager.BrowseDisplayMode.Name))
                {
                    foreach (WebZone zone in WPManager.Zones)
                    {
                        if (Request[zone.ID] != null)

                            if (Request[zone.ID] == "1")
                            {
                                zone.PartChromeType = PartChromeType.TitleAndBorder;
                                //EnsureWebPartForEdit(wpManager, zone, true);
                            }
                            else
                            {
                                zone.PartChromeType = PartChromeType.None;
                                //EnsureWebPartForEdit(wpManager, zone, false);
                            }
                    }

                    //SPContext.Current.Web.Update();
                }
            }
        }

        private void EnsureWebPartForEdit(SPWebPartManager wpManager, WebZone zone, bool p)
        {
            foreach (var wp in wpManager.WebParts)
            {
                var webPart = wp as WebPart;
                if (webPart.Zone.Equals(zone))
                    webPart.AllowEdit = p;
            }
        }

        #endregion

        protected Control FindControlRecursive(Control root, string id)
        {
            // best case ... root is the control we're looking for
            if (root.ID == id)
            {
                return root;
            }

            // better case ... root knows about control we're looking for
            if (root.FindControl(id) != null)
            {
                return root.FindControl(id);
            }

            // bad case ... recurse through root hierarchy to find control
            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
    }
}
