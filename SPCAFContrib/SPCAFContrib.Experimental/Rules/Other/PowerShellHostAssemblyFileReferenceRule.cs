using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Experimental.Rules.Other
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
        CheckId = CheckIDs.Rules.General.PowerShellHostAssemblyFileReferenceRule,
        DisplayName = "PS Host - AssemblyFileReference rule",
        Description = "PS Host - AssemblyFileReference rule",
        DefaultSeverity = Severity.CriticalWarning,
        SharePointVersion = new[] { "12", "14", "15" },
        Message = "String detected.",
        Resolution = ".")]
    public class PowerShellHostAssemblyFileReferenceRule : PowerShellHostBaseRule<AssemblyFileReference>
    {
        
    }
}
