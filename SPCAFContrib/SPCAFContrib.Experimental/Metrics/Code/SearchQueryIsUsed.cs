using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Metrics.Code.Base;

namespace SPCAFContrib.Experimental.Metrics.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Metrics.Assembly.SearchQueryIsUsed,
     DisplayName = "Search is used",
     Description = "It might be useful to have such an indication in the report for nont-IT people, IT pro and administrators. We need THE FACT that Search is used. It does not matter how much times, but we care that Search is used.",
     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Message = "",
     Resolution = "Setup search on the farm. Check if appropriate properties are created by solution, or by PS script or search used only with OOTB fields etc.",
     Links = new[]
     {
         "Using the SharePoint 2013 search Query APIs",
         "http://msdn.microsoft.com/en-us/library/office/dn423226(v=office.15).aspx",
     }
     )]
    public class SearchQueryIsUsed : SearchMethodMetricBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.KeywordQuery, new List<string>{
                    ".ctor"
                });

            TargetTypeMap.Add(TypeKeys.FullTextSqlQuery, new List<string>{
                    ".ctor"
                });
        }

        #endregion
    }
}
