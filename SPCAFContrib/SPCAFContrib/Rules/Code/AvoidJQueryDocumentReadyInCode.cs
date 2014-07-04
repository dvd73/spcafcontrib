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

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidJQueryDocumentReadyInCode,
     Help = CheckIDs.Rules.General.AvoidJQueryDocumentReady_HelpUrl,

     DisplayName = "Avoid using jQuery(document).ready in C# code.",
     Description = "Due to specific SharePoint client side initialization life cycle, it is recommended to avoid using jQuery(document).ready call.",
     Message = "jQuery(document).ready is used in method [{0}].",
     Resolution = "Use _spBodyOnLoadFunctions.push function or mQuery for SP2013.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" })]
    public class AvoidJQueryDocumentReadyInCode : Rule<AssemblyFileReference>
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
                    if (constValue.FindJQueryDocumentReadyByIndexOf())
                    {
                        CodeInstruction codeInstruction = new CodeInstruction(methodDefinition, instruction);
                        Notify(assembly, String.Format(this.MessageTemplate(), methodDefinition.FullName),
                            codeInstruction.ImproveSummary(assembly.GetSummary()), notifications);
                    }
                }
            }

            foreach (Mono.Cecil.ModuleDefinition moduleDefinition in assembly.AssemblyDefinition.Modules)
            {
                if (moduleDefinition.HasResources)
                {
                    foreach (EmbeddedResource resource in moduleDefinition.Resources.OfType<EmbeddedResource>())
                    {
                        byte[] resourceData = resource.GetResourceData();
                        string resValue = System.Text.UTF8Encoding.Default.GetString(resourceData);
                        if (!String.IsNullOrEmpty(resValue) && resValue.FindJQueryDocumentReadyByIndexOf())
                        {
                            Notify(assembly,
                                String.Format("jQuery(document).ready is used in assembly resource [{0}].", resource.Name),
                                assembly.GetSummary(), notifications);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
