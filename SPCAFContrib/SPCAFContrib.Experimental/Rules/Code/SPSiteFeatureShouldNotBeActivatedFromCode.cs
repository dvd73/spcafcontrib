using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Experimental.Rules.Code
{

    /// <summary>
    /// Duplicate SPCAF rule
    /// </summary>
    [RuleMetadata(typeof(ContribBestPracticesGroup),
        CheckId = CheckIDs.Rules.Feature.SPSiteFeatureShouldNotBeActivatedFromCode,
        Help = CheckIDs.Rules.Feature.SPSiteFeatureShouldNotBeActivatedFromCode_HelpUrl,

        Message = "SPFeature should not be activated via code.",
        DisplayName = "SPFeature should not be activated via code.",
        Description = "Avoid activation features via code; it requires unsafe updates/postbacks, creates unclear and hardly changeable activation sequence.",
        Resolution = "Consider deature activation depenency, manual activation via web interface or PowerShell script.",

        DefaultSeverity = Severity.Warning,
        SharePointVersion = new[] { "12", "14", "15" },
        Links = new[]
        {
            "https://spcafcontrib.codeplex.com/wikipage?title=http%3a%2f%2fmsdn.microsoft.com%2fen-us%2flibrary%2faa543162%28v%3doffice.14%29.aspx&referringTitle=CSC512101_SPSiteFeatureShouldNotBeActivatedFromCode",
            "https://spcafcontrib.codeplex.com/wikipage?title=http%3a%2f%2fmsdn.microsoft.com%2fen-us%2flibrary%2fee231535.aspx&referringTitle=CSC512101_SPSiteFeatureShouldNotBeActivatedFromCode"
        }
        )]
    public class SPSiteFeatureShouldNotBeActivatedFromCode : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPFeatureCollection, new List<string>{
                    "Add"
                });
        }

        #endregion
    }
}
