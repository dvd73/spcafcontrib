using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Metrics.Other
{
    [MetricMetadata(typeof(ContribMetricsGroup),
         CheckId = CheckIDs.Metrics.Assembly.NumberOfLayoutsPages,
         Help = CheckIDs.Metrics.Assembly.NumberOfLayoutsPages_HelpUrl,

         Message = "Solution [{0}] contains {1} layouts pages.",
         DisplayName = "Layouts pages count.",
         ShortName = "Layouts pages count.",
         Description = "Counts the number of the layouts pages within wsp package.",

         DefaultSeverity = Severity.Information,
         SharePointVersion = new[] { "12", "14", "15" },

         Unit = MetricUnit.Number,
         Aggregation = MetricAggregation.Sum
         )]
    public class NumberOfLayoutsPages : Metric<SolutionDefinition>
    {
        #region methods

        public override void Visit(SolutionDefinition solution, NotificationCollection notifications)
        {
            int numberOfLayoutsPages = 0;

            if (solution.TemplateFiles != null)
                numberOfLayoutsPages = solution
                                            .TemplateFiles
                                            .Count(t => !string.IsNullOrEmpty(t.Location) &&
                                                        t.Location.ToUpper().StartsWith("LAYOUTS") &&
                                                        t.Location.ToUpper().Contains(".ASPX"));

            Notify(solution, numberOfLayoutsPages, notifications, t => t.Name);
        }

        #endregion
    }
}
