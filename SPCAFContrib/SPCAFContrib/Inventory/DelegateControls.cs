using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk;
using SPCAF.Sdk.Inventory;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Controls.DelegateControls,
        Help = CheckIDs.Inventory.Controls.DelegateControls_HelpUrl,
        DisplayName = "Delegate controls",
        Description = "Provides information about delegate controls.",
        Message = "Added delegate control '{0}' to inventory",
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class DelegateControls : Inventory<DelegateControlDefinition>
    {
        protected string[] InventoryFields { get; set; }

        public DelegateControls()
        {
            InventoryFields = new string[] { "Id", "Sequence", "ControlClass", "ControlSrc" };
        }
        public override void Visit(DelegateControlDefinition target, NotificationCollection notifications)
        {
            string[] inventoryValues = new string[]
            {
                target.Id,target.SequenceSpecified ? target.Sequence.ToString() : String.Empty, target.ControlClass, target.ControlSrc
            };

            if (InventoryFields.Length > 0 && inventoryValues.Length > 0)
                Notify(target, InventoryFields, inventoryValues, String.Format(this.MessageTemplate(), target.Id), notifications);
        }
    }
}
