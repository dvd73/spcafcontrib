using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Inventory;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.ListOfStrings,
        Help = CheckIDs.Inventory.Assembly.ListOfStrings_HelpUrl,
        DisplayName = "Method strings",
        Description = "Provides information about all strings in the solution methods.",
        Message = "Added methods string '{0}' to inventory",
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class ListOfStrings : Inventory<AssemblyFileReference>
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            string[] inventoryFields = new string[] { "Assembly", "Class", "Method", "Value" };
            MultiValueDictionary<string, string> items = new MultiValueDictionary<string, string>();

            foreach (Mono.Cecil.MethodDefinition methodDefinition in assembly.AllMethodDefinitions().OrderBy(md => md.FullName))
            {
                foreach (Instruction instruction in methodDefinition.AllStringInstructions())
                {
                    string constValue = instruction.Operand != null ? instruction.Operand.ToString() : String.Empty;
                    items.TryAdd(methodDefinition.FullName, constValue);
                }
            }

            foreach (KeyValuePair<string, List<string>> item in items)
            {
                int lastDot = item.Key.LastIndexOf("::");
                int firstSpace = item.Key.IndexOf(" ");
                string methodDefinition_DeclaringType_FullName = item.Key.Substring(firstSpace + 1, lastDot - firstSpace - 1);
                string methodDefinition_Name = item.Key.Substring(lastDot + 2);

                foreach (string constValue in item.Value.OrderBy(s => s))
                {
                    string[] inventoryValues = new string[] {assembly.AssemblyName, methodDefinition_DeclaringType_FullName, methodDefinition_Name, constValue};
                    string message = String.Format(this.MessageTemplate(), constValue);
                    Notify(assembly, inventoryFields, inventoryValues, message, notifications);
                }
            }
        }
    }
}
