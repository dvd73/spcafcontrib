using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Inventory.Base;

namespace SPCAFContrib.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.CustomMasterPages,
        Help = CheckIDs.Inventory.Assembly.CustomMasterPages_HelpUrl,

        Message = "Added custom master page assigment [{0}] to inventory",
        DisplayName = "Custom master pages",
        Description = "Provides information about code-assigned custom master pages.",
        
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class CustomMasterPages : SearchPropertyInventoryBase
    {
        public CustomMasterPages()
        {
            InventoryFields = new string[] { "Assembly", "Type", "Value or Source" };
        }

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPWeb, new List<string> { "MasterUrl", "CustomMasterUrl" });
        }

        protected override void GetInventoryData(AssemblyFileReference assembly, CodeInstruction instruction, ref string[] inventoryValues, ref string message)
        {
            MemberReference m_ref = instruction.Instruction.Operand as MemberReference;

            Instruction prevInstruction = instruction.Instruction.Previous;
            string mp_Value = String.Empty;

            if (prevInstruction != null && prevInstruction.OpCode == OpCodes.Ldstr)
            {
                mp_Value = prevInstruction.Operand as string;
            }

            if (String.IsNullOrEmpty(mp_Value))
                mp_Value = !String.IsNullOrEmpty(instruction.FormattedForMessage) ? instruction.FormattedForMessage : String.Empty;

            if (m_ref != null)
            {
                if (String.Equals(m_ref.Name, "set_MasterUrl", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"Primary", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
                else if (String.Equals(m_ref.Name, "set_CustomMasterUrl", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"Secondary", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
            }
        }
    }
}
