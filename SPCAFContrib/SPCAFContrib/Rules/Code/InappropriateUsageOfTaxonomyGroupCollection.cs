using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Assembly.InappropriateUsageOfTaxonomyGroupCollection,
        Help = CheckIDs.Rules.Assembly.InappropriateUsageOfTaxonomyGroupCollection_HelpUrl,

        Message = "Inappropriate taxonomy collection usage.",
        DisplayName = "Inappropriate taxonomy collection usage.",
        Description = "Avoid string based index call on the taxonomy collection.",
        Resolution = "Consider fetching item by GUID or string comporation by collection enumeration.",

        DefaultSeverity = Severity.Warning,
        SharePointVersion = new[] { "14", "15" },
        Links = new []
        {
            "IndexedCollection<T>.Item property (Guid)",
            "http://msdn.microsoft.com/en-us/library/office/ee569459.aspx"
        }
        )]
    public class InappropriateUsageOfTaxonomyGroupCollection : Rule<AssemblyFileReference>
    {
        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<MethodDefinition> allMethods = assembly.AllMethodDefinitions();

            foreach (MethodDefinition method in allMethods)
            {
                foreach (Instruction termGroupCall in method.GetUnsafeTaxonomyTermStoreStringIndexCall())
                {
                    CodeInstruction instruction = new CodeInstruction(method, termGroupCall);
                    Notify(assembly, "Avoid string based index call on term store collection.",
                          instruction.ImproveSummary(assembly.GetSummary()),
                          notifications);
                }

                foreach (Instruction termGroupCall in method.GetUnsafeTaxonomyGroupStringIndexCall())
                {
                    CodeInstruction instruction = new CodeInstruction(method, termGroupCall);
                    Notify(assembly, "Avoid string based index call on term group collection.",
                          instruction.ImproveSummary(assembly.GetSummary()),
                          notifications);
                }

                foreach (Instruction termSetGroupCall in method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall())
                {
                    CodeInstruction instruction = new CodeInstruction(method, termSetGroupCall);
                    Notify(assembly, "Avoid string based index call on term set collection.",
                                instruction.ImproveSummary(assembly.GetSummary()),
                                notifications);
                }

                foreach (Instruction termSetGroupCall in method.GetUnsafeTaxonomyTermCollectionStringIndexCall())
                {
                    CodeInstruction instruction = new CodeInstruction(method, termSetGroupCall);
                    Notify(assembly, "Avoid string based index call on term collection.",
                                instruction.ImproveSummary(assembly.GetSummary()),
                                notifications);
                }
            }
        }

        #endregion
    }
}
