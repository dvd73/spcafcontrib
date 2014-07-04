using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
       CheckId = CheckIDs.Rules.Assembly.SPWebRequestAccessEmailPropertyUsage,
       Help = CheckIDs.Rules.Assembly.SPWebRequestAccessEmailPropertyUsage_HelpUrl,

       DisplayName = "SPWeb.RequestAccessEmail() method usage.",
       Message = "SPWeb.RequestAccessEmail() method usage.",
       Description = "SPWeb.RequestAccessEmail() method requires a proper 'Outgoing Email' farm settings. Ensure these configuration on the target farm.",
       Resolution = "SPWeb.RequestAccessEmail() method requires a proper 'Outgoing Email' farm settings. Ensure these configuration on the target farm.",

       DefaultSeverity = Severity.Information,
       SharePointVersion = new[] { "12", "14", "15" },
       Links = new []
       {
           "SPWeb.RequestAccessEmail property",
           "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spweb.requestaccessemail.aspx"
       }
       )]
    public class SPWebRequestAccessEmailPropertyUsage : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPWeb, new List<string>{
                    "set_RequestAccessEmail",
                    "get_RequestAccessEmail"
                });
        }

        #endregion
    }
}
