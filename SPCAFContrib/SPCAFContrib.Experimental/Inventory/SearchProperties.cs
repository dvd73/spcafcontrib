using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Inventory;
using SPCAF.Sdk.Model;
using Mono.Cecil.Cil;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Experimental.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.SearchProperties,
        Help = CheckIDs.Inventory.Assembly.SearchProperties_HelpUrl,
        DisplayName = "Search properties",
        Description = "Provides information about used search properties.",
        Message = "Added search property '{0}' to inventory",
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class SearchProperties : Inventory<AssemblyFileReference>
    {
        private static bool IsSP2013DefaultSearchProperty(string value)
        {
            // TODO
            return SP2013ManagedProperties.DefaultProperties.Count(p => string.Compare(p.Name, value, true) == 0) > 0;
        }

        private static bool IsSP2010DefaultSearchProperty(string value)
        {
            // TODO
            return SP2013ManagedProperties.DefaultProperties.Count(p => string.Compare(p.Name, value, true) == 0) > 0;
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            string spVersion = assembly.Parent.SharePointProductVersion;
            string[] inventoryFields = new string[] { "Assembly", "Name", "Is Default" };

            Dictionary<string, List<string>> targetTypeMethodMap = new Dictionary<string, List<string>>();

            targetTypeMethodMap.Add(TypeKeys.KeywordQuery, new List<string> {"get_SelectProperties"});

            foreach (string typeName in targetTypeMethodMap.Keys)
            {
                foreach (string methodName in targetTypeMethodMap[typeName])
                {
                    foreach (CodeInstruction methodCall in assembly.MethodInvocationInstructions(typeName, methodName))
                    {
                        Instruction nextInstruction = methodCall.Instruction.Next;

                        if (nextInstruction != null && nextInstruction.OpCode == OpCodes.Ldstr)
                        {
                            string propValue = nextInstruction.Operand as string;

                            if (!string.IsNullOrEmpty(propValue))
                            {
                                string[] inventoryValues = null;
                                string message = string.Format(this.MessageTemplate(), propValue);

                                switch (spVersion)
                                {
                                    case "14.0":
                                        {
                                            inventoryValues = new string[] {
                                                assembly.AssemblyName,
                                                propValue, 
                                                IsSP2010DefaultSearchProperty(propValue) ? "TRUE": "FALSE" 
                                            };
                                        }; break;
                                    case "15.0":
                                        {
                                            inventoryValues = new string[] {
                                                assembly.AssemblyName,
                                                propValue, 
                                                IsSP2013DefaultSearchProperty(propValue) ? "TRUE": "FALSE" 
                                            };
                                        }; break;
                                    default:
                                        {
                                            // TODO
                                            // update for SP2007
                                            inventoryValues = new string[] {
                                                assembly.AssemblyName,
                                                propValue, 
                                                "UNKNOWN" 
                                            };
                                        }; break;
                                }

                                Notify<AssemblyFileReference>(assembly, inventoryFields, inventoryValues, message, notifications);
                            }
                        }
                    }
                }
            }
        }
    }
}
