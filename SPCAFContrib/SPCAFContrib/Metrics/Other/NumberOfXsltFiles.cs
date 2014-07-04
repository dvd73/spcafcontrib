using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Metrics.Other
{
    [MetricMetadata(typeof(ContribMetricsGroup),
       CheckId = CheckIDs.Metrics.Assembly.NumberOfXsltFiles,
       Help = CheckIDs.Metrics.Assembly.NumberOfXsltFiles_HelpUrl,

       DisplayName = "Xsl/Xslt files count.",
       ShortName = "Xsl/Xslt files count.",
       Message = "Solution '{0}' contains {1} xslt/xsl files.",
       Description = "Counts the number of the xsl/xslt files wsp package.",

       DefaultSeverity = Severity.Information,
       SharePointVersion = new[] { "12", "14", "15" },

       Unit = MetricUnit.Number,
       Aggregation = MetricAggregation.Sum
       )]
    public class NumberOfXsltFiles : Metric<ModuleDefinition>
    {
        #region methods

        public override void Visit(ModuleDefinition module, NotificationCollection notifications)
        {
            int numerOfXsltFiles = module.GetFiles("xsl").Count();

            Notify(module, numerOfXsltFiles, notifications, t => t.Name);
        }

        #endregion
    }
}
