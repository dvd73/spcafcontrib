using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Common;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.OutOfContextSPWebPartManager,
     Help = CheckIDs.Rules.Assembly.OutOfContextSPWebPartManager_HelpUrl,

     Message = "Do not use SPWebPartManager when HTTPContext is null.",
     DisplayName = "Do not use SPWebPartManager when HTTPContext is null.",
     Description = "SharePoint supports an SPLimitedWebPartManager class that supports environments that have no HttpContext or Page available.",
     Resolution = " When no HttpContext (in event receivers for example) is available you should use SPLimitedWebPartManager.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPLimitedWebPartManager class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.webpartpages.splimitedwebpartmanager.aspx"
     })]
    public class OutOfContextSPWebPartManager : Rule<AssemblyFileReference>
    {
        MultiValueDictionary<string, string> MethodsAndItsSubCalls = new MultiValueDictionary<string, string>();
        
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

            OnMatch(assembly, notifications, assembly.MethodsUsingType(TypeKeys.SPWebPartManager));
        }

        private void OnMatch(AssemblyFileReference assembly, NotificationCollection notifications, IEnumerable<MethodDefinition> typeUsages)
        {
            foreach (MethodDefinition methodDefinition in typeUsages)
            {
                foreach (CodeInstruction instruction in methodDefinition.AllMethodReferences())
                {
                    MethodReference methodInvokation = instruction.Instruction.Operand as MethodReference;
                    if (methodInvokation != null)
                    {
                        if (String.Equals(methodInvokation.DeclaringType.FullName, TypeKeys.SPWebPartManager, StringComparison.OrdinalIgnoreCase) ||
                            String.Equals(methodInvokation.DeclaringType.FullName, TypeKeys.WebPartManager, StringComparison.OrdinalIgnoreCase))
                        {
                            NotifyIfOutOfContextCall(assembly, notifications, instruction, methodDefinition);
                        }
                    }
                }
            }
        }

        private void NotifyIfOutOfContextCall(AssemblyFileReference assembly, NotificationCollection notifications, CodeInstruction instruction,
            MethodDefinition methodDefinition)
        {
            TypeDefinition typeDefinition = instruction.MethodDefinition.DeclaringType;
            do
            {
                foreach (string fullTypeName in MethodsAndItsSubCalls.Keys)
                {
                    if (String.Equals(typeDefinition.FullName, fullTypeName) &&
                        MethodsAndItsSubCalls[fullTypeName].Contains(instruction.MethodDefinition.Name))
                    {
                        Notify(methodDefinition, this.MessageTemplate(),
                            instruction.ImproveSummary(assembly.GetSummary()), notifications);
                        return;
                    }
                }
                typeDefinition = typeDefinition.DeclaringType;
            } while (typeDefinition != null);
        }
    }
}
