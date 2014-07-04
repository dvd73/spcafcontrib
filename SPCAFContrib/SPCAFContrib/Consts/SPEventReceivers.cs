using System.Collections.Generic;

namespace SPCAFContrib.Consts
{
    public static partial class TypeInfo
    {
        #region properties

        public static readonly List<string> SPEventReceivers = new List<string>()
        {
            "Microsoft.SharePoint.SPItemEventReceiver",
            "Microsoft.SharePoint.SPListEventReceiver",
            "Microsoft.SharePoint.SPWebEventReceiver",
            "Microsoft.SharePoint.Workflow.SPWorkflowEventReceiver"
        };

        public static readonly List<string> SPItemEventReceiverAsynchronousEvents = new List<string>()
        {
            "ItemAdded",
            "ItemAttachmentAdded",
            "ItemAttachmentDeleted",
            "ItemCheckedIn",
            "ItemCheckedOut",
            "ItemDeleted",
            "ItemFileConverted",
            "ItemFileMoved",
            "ItemUncheckedOut",
            "ItemUpdated"
        };

        #endregion
    }
}
