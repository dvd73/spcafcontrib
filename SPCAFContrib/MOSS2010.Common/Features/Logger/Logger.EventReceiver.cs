using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebPartPages;
using MOSS.Common.Utilities;
using SharePoint.Common.Utilities.Extensions;
using WebPart = Microsoft.SharePoint.WebPartPages.WebPart;

namespace MOSS.Common.Features.Logger
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("7000f268-44f1-49a4-a366-4b967ac6269e")]
    public class LoggerEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var service = LoggingService.Instance;
            if (service != null)
            {
                service.Register();
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var service = LoggingService.Instance;
            if (service != null)
            {
                service.Unregister();
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

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
