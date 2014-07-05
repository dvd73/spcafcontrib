using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Metrics.Code
{
    [MetricMetadata(typeof(ContribMetricsGroup),
        CheckId = CheckIDs.Metrics.Assembly.NumberOfExternalDlls,
        Help = CheckIDs.Metrics.Assembly.NumberOfExternalDlls_HelpUrl,

        Message = "Solution [{0}] contains {1} external dlls within wsp package.",
        DisplayName = "External dll count.",
        ShortName = "External dll count.",
        Description = "Count external dll count in the solution.",

        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" },

        Unit = MetricUnit.Number,
        Aggregation = MetricAggregation.Sum
        )]
    public class NumberOfExternalDlls : Metric<SolutionDefinition>
    {
        #region methods

        public override void Visit(SolutionDefinition solution, NotificationCollection notifications)
        {
            int numberOfExternalDlls = 0;

            if (solution.Assemblies != null)
            {
                numberOfExternalDlls = solution
                                        .Assemblies
                                        .Count(assembly => !string.IsNullOrEmpty(assembly.Location) &&
                                                           !assembly.Location.Contains("System.") &&
                                                           !assembly.Location.Contains("Microsoft."));
            }

            Notify(solution, numberOfExternalDlls, notifications, t => t.Name);
        }

        #endregion
    }
}
