using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Experimental.Rules.Other
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
        CheckId = CheckIDs.Rules.General.PowerShellHostFeatureDefinitionRule,
        DisplayName = "PS Host - FeatureDefinition rule",
        Description = "PS Host - FeatureDefinition rule",
        DefaultSeverity = Severity.CriticalWarning,
        SharePointVersion = new[] { "12", "14", "15" },
        Message = "String detected.",
        Resolution = ".")]
    public class PowerShellHostFeatureDefinitionRule : PowerShellHostBaseRule<FeatureDefinition>
    {
    }
}
