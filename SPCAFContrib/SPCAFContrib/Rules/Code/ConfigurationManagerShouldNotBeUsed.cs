using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.ConfigurationManagerShouldNotBeUsed,
     Help = CheckIDs.Rules.Assembly.ConfigurationManagerShouldNotBeUsed_HelpUrl,

     DisplayName = "ConfigurationManager should not be used.",
     Message = "ConfigurationManager should not be used.",
     Description = "Due quite challenging web.config modification it might be a better choice to store config setting in SPFarm/SPWeb/SPList bag properties.",
     Resolution = "Consider SPFarm/SPWeb/SPList bag properties top store and manage configuration.",

     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SharePoint Guidance 2010 Hands On Lab",
         "https://spg.codeplex.com/",
         "Secure Store Service",
         "http://msdn.microsoft.com/en-us/library/ee557754.aspx",
         "Code Snippet: Get User Credentials Using the Default Secure Store Provider",
         "http://msdn.microsoft.com/en-us/library/ff394459.aspx"
     }
     )]
    public class ConfigurationManagerShouldNotBeUsed : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.ConfigurationManager, new List<string>{
                    "get_AppSettings",
                    "get_ConnectionStrings"
                });
        }

        #endregion
    }
}
