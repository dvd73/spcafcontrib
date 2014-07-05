using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Utilities;
using SPCAFContrib.Collectors;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Experimental.Rules.Code
{
    public class IncorrectDisposal : Rule<AssemblyFileReference>
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
            if (!hasSharePointReference) return;

            if (assembly.AssemblyHasExcluded()) return;

            foreach (Mono.Cecil.MethodDefinition methodDefinition in assembly.AllMethodDefinitions().Where(md => md.ReturnType.FullName == "Microsoft.SharePoint.SPWeb" || md.ReturnType.FullName == "Microsoft.SharePoint.SPSite"))
            {
                //TODO need to check Dispose() call for result variable
                var t = methodDefinition.DecompileMethodBody().GetText();
                
                //    Notify(assembly, this.MessageTemplate(), codeInstruction.ImproveSummary(assembly.GetSummary()), notifications);
            }
        }
    }
}
