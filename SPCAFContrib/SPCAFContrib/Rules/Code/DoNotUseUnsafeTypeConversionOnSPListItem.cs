using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAFContrib.Common;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotUseUnsafeTypeConversionOnSPListItem,
     Help = CheckIDs.Rules.Assembly.DoNotUseUnsafeTypeConversionOnSPListItem_HelpUrl,

     Message = "Do not use unsafe casts on SPListItem.",
     DisplayName = "Do not use unsafe casts on SPListItem.",
     Description = "SPListItem is untyped entity, so null reference exceptions on nullable types or wrong type conversion exception might arise.",
     Resolution = "Consider Convert.ToXXX method or manual conversion to handle wrong/nullable types.",

     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "Convert Methods",
         "http://msdn.microsoft.com/en-us/library/system.convert_methods(v=vs.110).aspx"
     }
     )]
    public class DoNotUseUnsafeTypeConversionOnSPListItem : Rule<AssemblyFileReference>
    {
        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<TypeDefinition> allTypes = assembly.AllTypeDefinitions();

            foreach (TypeDefinition type in allTypes)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    IEnumerable<Instruction> unsafeSPListItemCasts = method.GetUnsafeSPListItemCastInstructions();

                    foreach (Instruction instruction in unsafeSPListItemCasts)
                    {
                        Notify(assembly,
                               "Do not use unsafe casts on SPListItem.",
                               new CodeInstruction(method, instruction).ImproveSummary(assembly.GetSummary()),
                               notifications);
                    }
                }
            }
        }

        #endregion
    }
}
