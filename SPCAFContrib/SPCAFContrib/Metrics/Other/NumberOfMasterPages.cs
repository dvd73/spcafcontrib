using System.IO;
using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Metrics.Other
{
    [MetricMetadata(typeof(ContribMetricsGroup),
        CheckId = CheckIDs.Metrics.Assembly.NumberOfMasterPages,
        Help = CheckIDs.Metrics.Assembly.NumberOfMasterPages_HelpUrl,

        DisplayName = "Masterpage files count.",
        ShortName = "Masterpage files count.",
        Message = "Solution '{0}' contains {1} masterpage files.",
        Description = "Counts the number of the Masterpage files wsp package.",

        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" },

        Unit = MetricUnit.Number,
        Aggregation = MetricAggregation.Sum
        )]
    public class NumberOfMasterPages : Metric<ModuleDefinition>
    {
        #region methods

        public override void Visit(ModuleDefinition module, NotificationCollection notifications)
        {
            int result = 0;

            if (module.File != null)
            {
                result = module.File
                               .Count(file =>
                               {
                                   string extension = Path.GetExtension(file.Path);

                                   if (string.IsNullOrEmpty(extension)) return false;

                                   return extension.ToUpper().Contains("MASTER");
                               });
            }

            Notify(module, result, notifications, t => t.Name);
        }

        #endregion
    }
}
