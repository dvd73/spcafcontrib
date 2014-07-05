using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
       CheckId = CheckIDs.Rules.Assembly.AvoidHeavyOperationsInsideRepeaterItemEventHandlers,
       DisplayName = "AvoidHeavyOperationsInsideRepeaterItemEventHandlers",
       Description = "AvoidHeavyOperationsInsideRepeaterItemEventHandlers",
       DefaultSeverity = Severity.Warning,
       SharePointVersion = new[] { "12", "14", "15" },
       Message = "",
       Resolution = "AvoidHeavyOperationsInsideRepeaterItemEventHandlers")]
    public class AvoidHeavyOperationsInsideRepeaterItemEventHandlers : Rule<AssemblyFileReference>
    {
        #region contructors

        static AvoidHeavyOperationsInsideRepeaterItemEventHandlers()
        {
            HeavyOperations.Add("Microsoft.SharePoint.SPWeb", new List<string>()
            {
                "EnsureUser",
                "get_Lists"
            });
        }

        #endregion

        #region properties

        protected static Dictionary<string, List<string>> HeavyOperations = new Dictionary<string, List<string>>();

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<TypeDefinition> types = assembly.AllTypeDefinitions();

            foreach (TypeDefinition type in types)
            {
                Collection<MethodDefinition> methods = type.Methods;

                foreach (MethodDefinition method in methods)
                {
                    if (method.IsRepeaterItemEventHandler() || method.IsDataListItemEventHandler())
                    {
                        IEnumerable<Instruction> bindedInstructions = method.Body.Instructions
                                                            .Where(i => i.OpCode == OpCodes.Call ||
                                                                                        i.OpCode == OpCodes.Calli ||
                                                                                        i.OpCode == OpCodes.Callvirt);

                        foreach (Instruction bindedInstruction in bindedInstructions)
                        {
                            foreach (string heavyOperationClass in HeavyOperations.Keys)
                            {
                                foreach (string heavyOperationMethod in HeavyOperations[heavyOperationClass])
                                {
                                    if (bindedInstruction.Operand != null &&
                                        bindedInstruction.Operand.ToString().Contains(heavyOperationMethod))
                                    {
                                        Notify(assembly,
                                                   string.Format("AvoidHeavyOperationsInsideRepeaterItemEventHandlers. Method:[{0}]. Heavy operation:[{0}]", method.FullName, heavyOperationMethod),
                                                   new CodeInstruction(method, bindedInstruction).ImproveSummary(assembly.GetSummary()),
                                                   notifications);
                                    }
                                }
                            }
                        }



                    }
                }
            }
        }

        #endregion
    }
}
