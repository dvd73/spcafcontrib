using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.OutOfContextRWEP,
     Help = CheckIDs.Rules.Assembly.OutOfContextRWEP_HelpUrl,

     DisplayName = "Do not impersonate with RunWithElevatedPrivileges when HTTPContext is null.",
     Description = "You can't elevate privileges when using RunWithElevatedPrivileges in Workflow, Timer Job, Feature Receivers or Event handlers (asynchronous or not initiated by a request in browser). This rule is an addition for rule SPC020206.",
     Message = "Do not impersonate with RunWithElevatedPrivileges when HTTPContext is null.",
     Resolution = "Store credentials in SSS and use Win32 API to impersonate user.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPC020206: Avoid usage of 'RunWithElevatedPrivileges'",
         "http://docs.spcaf.com/v4/SPC020206_AvoidCallToRunWithElevatedPrivileges.html",
         "Impersonation in SharePoint : An Extreme Overview",
         "http://extreme-sharepoint.com/2012/05/30/impersonation-elevation-of-privileges",
         "Sample of impersonation not using RunWithElevatedPrivilages",
         "http://www.sharepoint-tips.com/2007/03/sample-event-handler-to-set-permissions.html",
         "Impersonation in Event Handlers",
         "http://www.sharepoint-tips.com/2007/03/impersonation-in-event-handlers.html",
         "Event handler impersonation - continued",
         "http://www.sharepoint-tips.com/2007/03/event-handler-impersonation-continued.html",
         "SharePoint Feature Receivers – the hidden details",
         "http://blog.pentalogic.net/2010/06/sharepoint-feature-receivers-events-details"
     })]
    public class OutOfContextRWEP : SearchMethodRuleBase
    {
        MultiValueDictionary<string, string> MethodsAndItsSubCalls = new MultiValueDictionary<string, string>();

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPSecurity, "RunWithElevatedPrivileges");
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

            base.Visit(assembly, notifications);
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
