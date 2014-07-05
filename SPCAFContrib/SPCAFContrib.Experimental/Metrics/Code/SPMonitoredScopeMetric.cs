using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Metrics;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Experimental.Metrics.Code
{
    [MetricMetadata(typeof(ContribMetricsGroup),
       CheckId = CheckIDs.Metrics.Assembly.SPMonitoredScopeMetric,
       Help = CheckIDs.Metrics.Assembly.SPMonitoredScopeMetric_HelpUrl,
       DisplayName = "SPMonitoredScope usage count",
       ShortName = "SPMonitoredScope usage count",
       Description = "Counts of usage SPMonitoredScope construction.",
       Message = "Solution '{0}' contains {1} SPMonitoredScope usages.",
       SharePointVersion = new[] { "12", "14", "15" },
       Unit = MetricUnit.Number,
       Aggregation = MetricAggregation.Sum)]
    public class SPMonitoredScopeMetric : Metric<AssemblyFileReference>
    {

        #region Public Methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            int result = 0;
            IEnumerable<MethodDefinition> methods = assembly.AllMethodDefinitions();

            foreach (MethodDefinition method in methods)
            {
                result += method.GetSPMonitoredScopeUsageCount();
            }

            Notify(assembly, result, notifications);
        }

        #endregion

    }
}
