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
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Experimental.Inventory
{
    [InventoryMetadata(typeof(ContribInventoryGroup),
        CheckId = CheckIDs.Inventory.Assembly.UserProfileProperties,
        Help = CheckIDs.Inventory.Assembly.UserProfileProperties_HelpUrl,
        DisplayName = "User profile properties",
        Description = "Provides information about used user profile properties used.",
        Message = "Added user profile property '{0}' to inventory",
        SharePointVersion = new string[] { "12", "14", "15" })]
    public class UserProfileProperties : Inventory<AssemblyFileReference>
    {
        private static bool IsSP2013DefaultUserProperty(string value)
        {
            // TODO
            // needs to be checked if 2013 has new props
            return SP2010UserProfileProperties.DefaultProperties.Count(p => string.Compare(p.Name, value, true) == 0) > 0;
        }

        private static bool IsSP2010DefaultUserProperty(string value)
        {
            return SP2010UserProfileProperties.DefaultProperties.Count(p => string.Compare(p.Name, value, true) == 0) > 0;
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            string spVersion = assembly.Parent.SharePointProductVersion;

            string[] inventoryFields = new string[] { "Assembly", "Name", "Is Default" };
            Dictionary<string, List<string>> targetTypeMethodMap = new Dictionary<string, List<string>>();

            targetTypeMethodMap.Add(TypeKeys.UserProfile, new List<string> {"get_Item"});

            foreach (string typeName in targetTypeMethodMap.Keys)
            {
                foreach (string methodName in targetTypeMethodMap[typeName])
                {
                    foreach (CodeInstruction methodCall in assembly.MethodInvocationInstructions(typeName, methodName))
                    {
                        Instruction prevInstruction = methodCall.Instruction.Previous;

                        if (prevInstruction != null && prevInstruction.OpCode == OpCodes.Ldstr)
                        {
                            string propValue = prevInstruction.Operand as string;

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
                                                IsSP2010DefaultUserProperty(propValue) ? "TRUE": "FALSE" 
                                            };
                                        }; break;
                                    case "15.0":
                                        {
                                            inventoryValues = new string[] {
                                                assembly.AssemblyName,
                                                propValue, 
                                                IsSP2010DefaultUserProperty(propValue) ? "TRUE": "FALSE" 
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
