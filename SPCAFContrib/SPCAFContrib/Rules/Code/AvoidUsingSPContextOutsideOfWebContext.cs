using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUsingSPContextOutsideOfWebContext,
     Help = CheckIDs.Rules.Assembly.AvoidUsingSPContextOutsideOfWebContext_HelpUrl,

     DisplayName = "Avoid using SPContext.Current outside of web request context.",
     Description = "Avoid using SPContext.Current outside of web request context. ",
     Message = "Avoid using SPContext.Current outside of web request context. Method: [{0}], Class:[{1}]",
     Resolution = "",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPContext.Current property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spcontext.current.aspx"
     })]
    public class AvoidUsingSPContextOutsideOfWebContext : SearchPropertyRuleBase
    {
        MultiValueDictionary<string, string> MethodsAndItsSubCalls = new MultiValueDictionary<string, string>();

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPContext, "Current");
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            // get generic types
            List<TypeDefinition> types =
                assembly.AllTypeDefinitions()
                    .Where(
                        td =>
                            (td.BaseType != null &&
                             (td.BaseType.Namespace == "System.Activities" ||
                              td.BaseType.Namespace == "System.ServiceModel.Activities") &&
                             td.BaseType.IsGenericInstance))
                    .ToList();
            types.AddRange(assembly.ResolveOutOfHttpContextTypes());

            foreach (TypeDefinition typeDefinition in types)
            {
                HashSet<string> handledMethods = new HashSet<string>();
                typeDefinition.InsideMethodCalls(MethodsAndItsSubCalls, handledMethods, true, null);
            }

            EnsureTypeMap();

            assembly.OnPropertyUsageMatch(TargetTypeMap, (_instruction) =>
            {
                this.OnMatch(assembly, _instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, _instruction);
                });
            });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications, Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            if (instruction.MethodDefinition.DeclaringType != null)
            {
                TypeDefinition typeDefinition = instruction.MethodDefinition.DeclaringType;
                do
                {
                    foreach (string fullTypeName in MethodsAndItsSubCalls.Keys)
                    {
                        if (String.Equals(typeDefinition.FullName, fullTypeName) && 
                            MethodsAndItsSubCalls[fullTypeName].Contains(instruction.MethodDefinition.Name))
                        {
                            base.OnMatch(assembly, instruction, notifications, 
                                () =>
                            {
                                return String.Format(this.MessageTemplate(), instruction.MethodDefinition.Name, fullTypeName);
                            }, () =>
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
