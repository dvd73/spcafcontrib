using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Inventory.Base;

namespace SPCAFContrib.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.QueryStringUsage,
        Help = CheckIDs.Inventory.Assembly.QueryStringUsage_HelpUrl,
        DisplayName = "HttpRequest collections monitoring",
        Description = "Provides information about HttpRequest key collections usage.",
        Message = "Added HttpRequest collection key usage '{0}' to inventory",
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class HttpRequestCollectionsUsage : SearchPropertyInventoryBase
    {
        public HttpRequestCollectionsUsage()
        {
            InventoryFields = new string[] { "Assembly", "Collection", "Parameter or Method" };
        }

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.HttpRequest, new List<string> { "QueryString", "Form", "Cookies", "ServerVariables", "Item" });
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);
            
            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnPropertyUsageMatch(TargetTypeMap, (_methodInstruction) =>
            {
                this.OnMatch(assembly, _methodInstruction, notifications);
            });
        }

        protected override void GetInventoryData(AssemblyFileReference assembly, CodeInstruction instruction, ref string[] inventoryValues,
            ref string message)
        {
            MemberReference m_ref = instruction.Instruction.Operand as MemberReference;

            Instruction prevInstruction = instruction.Instruction.Previous;
            Instruction nextInstruction = instruction.Instruction.Next;
            string mp_Value = String.Empty;

            if (prevInstruction != null && prevInstruction.OpCode == OpCodes.Ldstr)
            {
                mp_Value = prevInstruction.Operand as string;
            }

            if (String.IsNullOrEmpty(mp_Value))
            {
                if (nextInstruction != null && nextInstruction.OpCode == OpCodes.Ldstr)
                {
                    mp_Value = nextInstruction.Operand as string;
                }
            }

            if (String.IsNullOrEmpty(mp_Value))
                mp_Value = !String.IsNullOrEmpty(instruction.FormattedForMessage) ? instruction.FormattedForMessage : String.Empty;

            if (m_ref != null)
            {
                if (String.Equals(m_ref.Name, "get_QueryString", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"QueryString", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
                else if (String.Equals(m_ref.Name, "get_Form", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"Form", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
                else if (String.Equals(m_ref.Name, "get_Cookies", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"Cookies", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
                else if (String.Equals(m_ref.Name, "get_ServerVariables", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"ServerVariables", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
                else if (String.Equals(m_ref.Name, "get_Item", StringComparison.CurrentCultureIgnoreCase))
                {
                    inventoryValues = new string[]
                    {
                        assembly.AssemblyName,"Any", mp_Value
                    };
                    message = string.Format(this.MessageTemplate(), mp_Value);
                }
            }
        }
    }
}
