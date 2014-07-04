using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Metrics.Code.Base;

namespace SPCAFContrib.Metrics.Code
{
     [MetricMetadata(typeof(ContribMetricsGroup),
        CheckId = CheckIDs.Metrics.Assembly.NumberOfBreakInheritances,
        Help = CheckIDs.Metrics.Assembly.NumberOfBreakInheritances_HelpUrl,

        DisplayName = "BreakInheritance/ResetRoleInheritance call count.",
        ShortName = "BreakInheritance/ResetRoleInheritance calls.",
        Message = "Solution '{0}' contains {1} BreakInheritance/ResetRoleInheritance calls within wsp package.",
        Description = "Count of BreakInheritance/ResetRoleInheritance calls in the solution.",

        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" },

        Unit = MetricUnit.Number,
        Aggregation = MetricAggregation.Sum
        )]
    public class NumberOfBreakInheritance : SearchMethodMetricBase
    {
         protected override void PopulateTypeMap()
         {
             List<string> methods =  new List<string> {"BreakRoleInheritance", "ResetRoleInheritance"};

             TargetTypeMap.Add(TypeKeys.SPSecurableObject, methods);
             TargetTypeMap.Add(TypeKeys.SPRoleDefinitionCollection, methods);
         }
    }
}
