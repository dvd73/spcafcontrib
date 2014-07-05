using System;
using System.Collections.Generic;
using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Collectors;
using SPCAFContrib.Common;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using Severity = SPCAF.Sdk.Severity;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.ULSLoggingShouldBeUsed,
     Help = CheckIDs.Rules.Assembly.ULSLoggingShouldBeUsed_HelpUrl,

     Message = "ULS logging should be used in assembly [{0}]",
     DisplayName = "Assembly does not contains ULS logging.",
     Description = "ULS logging should be used in the SharePoint assembly",
     Resolution = "Use SPDiagnosticsService class to log.",

     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPDiagnosticsService class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.administration.spdiagnosticsservice.aspx"
     })]
    public class ULSLoggingShouldBeUsed : SearchMethodRuleBase
    {
        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            SolutionDefinition solution = assembly.ParentSolution as SolutionDefinition;

            bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
            if (!hasSharePointReference) return;

            if (assembly.AssemblyHasExcluded()) return;

            if (!LoggingStatistic.Instance.Collected)
            {
                new LoggingCollector().Visit(solution);    
            }

            if (LoggingStatistic.Instance.Collected)
            {
                bool b = assembly.AllMethodDefinitions().Any(md => HasCustomLoggerCall(md) || HasSPDiagnosticsServiceCall(md));
                
                if (!b)
                    Notify(assembly, String.Format(this.MessageTemplate(), assembly.AssemblyName), assembly.GetSummary(), notifications);
            }
        }

        private bool HasCustomLoggerCall(MethodDefinition methodDefinition)
        {
            MultiValueDictionary<string, string> MethodSubCalls = new MultiValueDictionary<string, string>();
            HashSet<string> handledMethods = new HashSet<string>();
            methodDefinition.InsideMethodsCalls(MethodSubCalls, handledMethods, true, _ => { });

            return LoggingStatistic.Instance.CustomTypes.Any(ct => MethodSubCalls.ContainsKey(ct));
        }

        private bool HasSPDiagnosticsServiceCall(MethodDefinition methodDefinition)
        {
            MultiValueDictionary<string, string> MethodSubCalls = new MultiValueDictionary<string, string>();
            HashSet<string> handledMethods = new HashSet<string>();
            methodDefinition.InsideMethodsCalls(MethodSubCalls, handledMethods, false, _ => { });

            return MethodSubCalls.ContainsKey(TypeKeys.SPDiagnosticsService);
        }

        protected override void PopulateTypeMap()
        {
            throw new NotImplementedException();
        }
    }
}
