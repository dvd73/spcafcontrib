using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
    CheckId = CheckIDs.Rules.Assembly.AvoidInlineJavaScriptCode,
    DisplayName = "AvoidInlineJavaScriptCode",
    Description = "AvoidInlineJavaScriptCode",
    DefaultSeverity = Severity.Information,
    SharePointVersion = new[] { "12", "14", "15" },
    Message = "JavaScript should not be inlined in the code. Consider separation into *.js file. Detected string:[{0}]",
    Resolution = "AvoidInlineJavaScriptCode")]
    public class AvoidInlineJavaScriptCode : Rule<AssemblyFileReference>
    {
        #region properties

        private const string JavaScriptTag = "javascript:";

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            Action<Mono.Cecil.MethodDefinition, Instruction> methodStringCallback = (method, instruction) =>
            {
                string value = instruction.Operand as string;

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.IndexOf(JavaScriptTag, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        Notify(assembly,
                           string.Format(this.MessageTemplate(), value),
                           new CodeInstruction(method, instruction).ImproveSummary(assembly.GetSummary()),
                           notifications);
                    }
                }
            };

            CachedOperationExtensions.SearchMethodStringInstructions(assembly, methodStringCallback);
        }

        #endregion
    }
}
