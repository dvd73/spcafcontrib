using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;
using SPCAFContrib.Extensions;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Assembly.InappropriateUsageOfTaxonomyGroupCollection,
        Help = CheckIDs.Rules.Assembly.InappropriateUsageOfTaxonomyGroupCollection_HelpUrl,

        DisplayName = "Inappropriate taxonomy collection usage.",
        Message = "Inappropriate taxonomy collection usage.",
        Description = "Avoid string based index call on term group/term set collection.",
        Resolution = "Consider fetching term group/term set by GUID or string comporation by collection enumeration.",

        DefaultSeverity = Severity.Error,
        SharePointVersion = new[] { "14", "15" },
        Links = new []
        {
            "IndexedCollection<T> class",
            "http://msdn.microsoft.com/en-us/library/ee574147.aspx",
            "Group class",
            "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.taxonomy.group.aspx"
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
                foreach (Instruction termGroupCall in method.GetUnsafeTaxonomyGroupStringIndexCall())
                {
                    CodeInstruction instruction = new CodeInstruction(method, termGroupCall);
                    Notify(assembly, "Avoid string based index call on term group.",
                          instruction.ImproveSummary(assembly.GetSummary()),
                          notifications);
                }

                foreach (Instruction termSetGroupCall in method.GetUnsafeTaxonomyTermSetCollectionStringIndexCall())
                {
                    CodeInstruction instruction = new CodeInstruction(method, termSetGroupCall);
                    Notify(assembly, "Avoid string based index call on term set.",
                                instruction.ImproveSummary(assembly.GetSummary()),
                                notifications);
                }
            }
        }

        #endregion
    }
}
