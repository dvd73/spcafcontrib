using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
       CheckId = CheckIDs.Rules.Assembly.SPWebEnsureUserMethodUsage,
       Help = CheckIDs.Rules.Assembly.SPWebEnsureUserMethodUsage_HelpUrl,

       Message = "Tend to reduce SPWeb.EnsureUser() method usage.",
       DisplayName = "SPWeb.EnsureUser() method usage.",
       Description = "Tend to reduce SPWeb.EnsureUser() method usage as it is quite costly operation.",
       Resolution = "Tend to reduce SPWeb.EnsureUser() method usage as it is quite costly operation.",

       DefaultSeverity = Severity.Information,
       SharePointVersion = new[] { "12", "14", "15" },
       Links = new []
       {
           "SPWeb.EnsureUser method",
           "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spweb.ensureuser.aspx"
       }
       )]
    public class SPWebEnsureUserMethodUsage : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPWeb, new List<string>{
                    "EnsureUser"
                });
        }

        #endregion
    }
}
