using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Metrics;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Metrics.Other
{
    [MetricMetadata(typeof(ContribMetricsGroup),
         CheckId = CheckIDs.Metrics.Assembly.NumberOfControlTemplates,
         Help = CheckIDs.Metrics.Assembly.NumberOfControlTemplates_HelpUrl,

         DisplayName = "Control templates count.",
         ShortName = "Control templates count.",
         Message = "Solution '{0}' contains {1} control templates.",
         Description = "Counts the number of control templates with in wsp package.",

         DefaultSeverity = Severity.Information,
         SharePointVersion = new[] { "12", "14", "15" },

         Unit = MetricUnit.Number,
         Aggregation = MetricAggregation.Sum
         )]
    public class NumberOfControlTemplates : Metric<SolutionDefinition>
    {
        #region methods

        public override void Visit(SolutionDefinition solution, NotificationCollection notifications)
        {
            int numberOfControlTemplates = 0;

            if (solution.TemplateFiles != null)
                numberOfControlTemplates = solution
                                             .TemplateFiles
                                             .Count(template => !string.IsNullOrEmpty(template.Location) &&
                                                                template.Location.ToUpper().StartsWith("CONTROLTEMPLATES"));

            Notify(solution, numberOfControlTemplates, notifications, t => t.Name);
        }

        #endregion
    }
}
