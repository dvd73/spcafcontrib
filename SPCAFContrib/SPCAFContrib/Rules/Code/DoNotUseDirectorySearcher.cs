using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotUseDirectorySearcher,
     Help = CheckIDs.Rules.Assembly.DoNotUseDirectorySearcher_HelpUrl,

     DisplayName = "Do not use DirectorySearcher class to query ActiveDirectory.",
     Message = "Consider SPUtility.GetPrincipalsInGroup to get things done.",
     Description = "DirectorySearcher might not work well across multiple domains and required additional security configuration.",
     Resolution = "Consider SPUtility.GetPrincipalsInGroup method to perform necessary queries.",

     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPUtility.GetPrincipalsInGroup method",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.utilities.sputility.getprincipalsingroup.aspx"
     }
     )]
    public class DoNotUseDirectorySearcher : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.DirectorySearcher, new List<string>{
                    ".ctor"
                });
        }

        #endregion
    }
}
