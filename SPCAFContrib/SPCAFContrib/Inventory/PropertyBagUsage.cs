using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Inventory.Base;

namespace SPCAFContrib.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.PropertyBagUsage,
        Help = CheckIDs.Inventory.Assembly.PropertyBagUsage_HelpUrl,
        DisplayName = "Property bag stores custom value",
        Description = "Provides information about Property bag usage as values storage.",
        Message = "Added property bag instruction '{0}' to inventory",
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class PropertyBagUsage: SearchMethodInventoryBase
    {
        public PropertyBagUsage()
        {
            InventoryFields = new string[] { "Assembly", "Source" };
        }

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPPropertyBag, new List<string> { "Update"});
        }

        protected override void GetInventoryData(AssemblyFileReference assembly, CodeInstruction instruction, ref string[] inventoryValues, ref string message)
        {
            MemberReference m_ref = instruction.Instruction.Operand as MemberReference;
            string mp_Value = !String.IsNullOrEmpty(instruction.FormattedForMessage) ? instruction.FormattedForMessage : String.Empty;

            if (m_ref != null)
            {
                inventoryValues = new string[]
                {
                    assembly.AssemblyName, mp_Value
                };
                message = string.Format(this.MessageTemplate(), mp_Value);
            }
        }
    }
}
