using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Common;
using SPCAF.Sdk;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Assembly.MagicStringShouldNotBeUsed,
        Help = CheckIDs.Rules.Assembly.MagicStringShouldNotBeUsed_HelpUrl,

        Message = "Hardcoded {1} is detected \"{0}\".",
        DisplayName = "Do not use hardcoded urls, pathes, emails and account names in code.",
        Description = "Consider a configuration for solution.",
        Resolution = "Consider a configuration for solution.",

        DefaultSeverity = Severity.Warning,
        SharePointVersion = new[] { "12", "14", "15" }
        )]
    public class MagicStringShouldNotBeUsed : Rule<AssemblyFileReference>
    {
        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            var magicStrings = from method in assembly.AllMethodDefinitions()
                               where method.HasBody
                               let body = method.Body
                               from instruction in body.Instructions
                               where instruction.OpCode == OpCodes.Ldstr
                               let value = instruction.Operand as string
                               let match = MagicStringsHelper.Match(value)
                               where match != null
                               select new
                               {
                                   Value = value,
                                   Match = match,
                                   Instruction = new CodeInstruction(method, instruction)
                               };

            foreach (var item in magicStrings)
            {
                this.Notify(assembly,
                               string.Format(this.MessageTemplate(), item.Value, item.Match),
                               item.Instruction.ImproveSummary(assembly.GetSummary()),
                               notifications);
            }

        }
        #endregion
    }
}
