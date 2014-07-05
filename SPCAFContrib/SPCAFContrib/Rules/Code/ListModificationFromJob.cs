using System;
using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.ListModificationFromJob,
     Help = CheckIDs.Rules.Assembly.ListModificationFromJob_HelpUrl,

     Message = "Workflow does not start if list modified from timer job.",
     DisplayName = "Workflow does not start if list modified from timer job.",
     Description = "When you perform list item Create/Update/Detele operation from timer job it does not raise related workflows because job works under pool (system) account.",
     Resolution = "It is required to start workflows from code in the list receiver.",

     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPWorkflowManager class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.workflow.spworkflowmanager.aspx"
     })]
    public class ListModificationFromJob : SearchMethodRuleBase
    {
        MultiValueDictionary<string, string> JobMethodsAndItsSubCalls = new MultiValueDictionary<string, string>();

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPList, new List<string>{
                    "AddItem"
                });
            TargetTypeMap.Add(TypeKeys.SPListCollection, new List<string>{
                    "AddItem"
                });
            TargetTypeMap.Add(TypeKeys.SPListItem, new List<string>{
                    "Delete", "Update", "SystemUpdate"
                });
            TargetTypeMap.Add(TypeKeys.SPFile, new List<string>{
                    "Delete","Update"
                }); 
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);
            
            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<TypeDefinition> types = assembly.ResolveTypesByBaseTypes(TypeInfo.SPTimerJobs);
            foreach (TypeDefinition typeDefinition in types)
            {
                HashSet<string> handledMethods = new HashSet<string>();
                typeDefinition.InsideMethodCalls(JobMethodsAndItsSubCalls, handledMethods, true, _ =>{});
            }

            base.Visit(assembly, notifications);
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications, Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            if (instruction.MethodDefinition.DeclaringType != null)
            {
                TypeDefinition typeDefinition = instruction.MethodDefinition.DeclaringType;
                do
                {
                    foreach (string fullTypeName in JobMethodsAndItsSubCalls.Keys)
                    {
                        if (String.Equals(typeDefinition.FullName, fullTypeName))
                        {
                            base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                            {
                                return GetSummary(assembly, instruction);
                            });
                            return;
                        }
                    }
                    typeDefinition = typeDefinition.DeclaringType;
                }
                while (typeDefinition != null);
            }
        }
    }
}
