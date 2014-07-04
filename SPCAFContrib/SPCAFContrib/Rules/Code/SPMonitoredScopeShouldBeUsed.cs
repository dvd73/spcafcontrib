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
using SPCAFContrib.Extensions;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
       CheckId = CheckIDs.Rules.Assembly.SPMonitoredScopeShouldBeUsed,
       Help = CheckIDs.Rules.Assembly.SPMonitoredScopeShouldBeUsed_HelpUrl,
       DisplayName = "SPMonitoredScope should be used for web parts, pages and controls.",
       Description = "Some recommended practices regarding SPMonitoredScope class utilization.",
       DefaultSeverity = Severity.Information,
       SharePointVersion = new[] { "12", "14", "15" },
       Message = "You don't use SPMonitoredScope construction in the visual component [{0}].",
       Resolution = "Consider to use SPMonitoredScope in visual components.",
       Links = new []
       {
           "Using SPMonitoredScope",
           "http://msdn.microsoft.com/en-us/library/ff512758.aspx"
       })]

    public class SPMonitoredScopeShouldBeUsed : Rule<AssemblyFileReference>
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;
            if (!assembly.AssemblyReferencesSharePointAssembly()) return;

            HashSet<TypeDefinition> allowedTypes = new HashSet<TypeDefinition>(assembly.ResolveAllowedWebControls());
            
            foreach (TypeDefinition typeDefinition in allowedTypes)
            {
                if (typeDefinition.IsDerivedFromType("System.Web.UI.Page") ||
                    typeDefinition.IsDerivedFromType("System.Web.UI.MasterPage")) continue;

                bool hasSPMonitoredScope = false;
                MultiValueDictionary<string, string> methodList = new MultiValueDictionary<string, string>();
                HashSet<string> handledMethods = new HashSet<string>();
                typeDefinition.InsideMethodCalls(methodList, handledMethods, true, (method) =>
                {
                    hasSPMonitoredScope |= method.HasSPMonitoredScope();
                });

                if (!hasSPMonitoredScope)
                    Notify(assembly, String.Format(this.MessageTemplate(), typeDefinition.Name), typeDefinition.ImproveSummary(assembly.GetSummary()), notifications);
            }
        }
    }
}
