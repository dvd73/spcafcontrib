using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
       CheckId = CheckIDs.Rules.Assembly.AvoidStringIndexerInSPViewCollection,
       DisplayName = "Avoid string indexer in SPViewCollection",
       Description = "Avoid string indexer in SPViewCollection. It is case sensitive so that it leads to potential issue in the production.",
       DefaultSeverity = Severity.Warning,
       SharePointVersion = new[] { "12", "14", "15" },
       Message = "",
       Resolution = "Avoid string indexer in SPViewCollection")]
    public class AvoidStringIndexerInSPViewCollection : SearchPropertyRuleBase
    {
        protected override void PopulateTypeMap()
        {
            // TODO, add right method name here for the string indexer
            //TargetTypeMap.Add(TypeKeys.SPViewCollection, new List<string> { "." });
        }
    }
}
