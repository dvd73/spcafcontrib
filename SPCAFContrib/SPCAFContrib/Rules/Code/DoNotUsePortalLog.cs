using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotUsePortalLog,
     Help = CheckIDs.Rules.Assembly.DoNotUsePortalLog_HelpUrl,

     Message = "Do not use Microsoft.Office.Server.Diagnostics.PortalLog.",
     DisplayName = "Do not use Microsoft.Office.Server.Diagnostics.PortalLog.",
     Description = "This class and its members are reserved for internal use and are not intended to be used in your code.",
     Resolution = "Use SPDiagnosticsServiceBase class for log.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "14", "15" },
     Links = new[]
     {
         "SPDiagnosticsServiceBase class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.administration.spdiagnosticsservicebase.aspx"
     })]
    public class DoNotUsePortalLog : Rule<AssemblyFileReference>
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
            if (!hasSharePointReference) return;

            if (assembly.AssemblyHasExcluded()) return;

            foreach (Mono.Cecil.MethodDefinition methodDefinition in assembly.MethodsUsingType(TypeKeys.PortalLog))
            {
                foreach (CodeInstruction codeInstruction in methodDefinition.AllMethodReferences().Where(ci => HasPortalLogCall(ci)))
                {
                    Notify(assembly, this.MessageTemplate(), codeInstruction.ImproveSummary(assembly.GetSummary()), notifications);   
                }
            }
        }

        private bool HasPortalLogCall(CodeInstruction codeInstruction)
        {
            MethodReference methodReference = codeInstruction.Instruction.Operand as MethodReference;
            if (methodReference != null)
                return methodReference.DeclaringType.FullName.Contains(TypeKeys.PortalLog);
            
            return true;
        }
    }
}
