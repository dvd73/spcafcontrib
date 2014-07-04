using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidDollarGlobalVariableInCode,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,
     
     DisplayName = "Avoid using $ for jQuery in C# code.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Message = "jQuery $ variable is used in method [{0}].",
     Resolution = "Use jQuery global variable instead of $.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" })]
    public class AvoidDollarGlobalVariableInCode : Rule<AssemblyFileReference>
    {
        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            foreach (Mono.Cecil.MethodDefinition methodDefinition in assembly.AllMethodDefinitions().OrderBy(md => md.FullName))
            {
                foreach (Instruction instruction in methodDefinition.AllStringInstructions())
                {
                    string constValue = instruction.Operand != null ? instruction.Operand.ToString() : String.Empty;
                    if (constValue.FindJQueryVariableByIndexOf())
                    {
                        CodeInstruction codeInstruction = new CodeInstruction(methodDefinition, instruction);
                        Notify(assembly, String.Format(this.MessageTemplate(), methodDefinition.FullName),
                            codeInstruction.ImproveSummary(assembly.GetSummary()), notifications);
                    }
                }
            }
            
            foreach (ModuleDefinition moduleDefinition in assembly.AssemblyDefinition.Modules)
            {
                if (moduleDefinition.HasResources)
                {
                    foreach (EmbeddedResource resource in moduleDefinition.Resources.OfType<EmbeddedResource>())
                    {
                        byte[] resourceData = resource.GetResourceData();
                        string resValue = System.Text.UTF8Encoding.Default.GetString(resourceData);
                        if (!String.IsNullOrEmpty(resValue) && resValue.FindJQueryVariableByIndexOf())
                        {
                            Notify(assembly,
                                String.Format("jQuery $ variable is used in assembly resource [{0}].", resource.Name),
                                assembly.GetSummary(), notifications);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
