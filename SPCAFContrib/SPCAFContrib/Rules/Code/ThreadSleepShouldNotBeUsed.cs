using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;
using System.Collections.Generic;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Assembly.ThreadSleepShouldNotBeUsed,
        Help = CheckIDs.Rules.Assembly.ThreadSleepShouldNotBeUsed_HelpUrl,

        DisplayName = "Thread.Sleep() method should not be used.",
        Message = "Thread.Sleep() method should not be used.",
        Description = "Usually, Thread.Sleep() indicates lack of the general design or misunderstanding of SharePoint API.",
        Resolution = "Review the code, it might require additional attention or redesign.",

        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" }
        )]
    public class ThreadSleepShouldNotBeUsed : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.Thread, new List<string>{
                    "Sleep"
                });
        }

        #endregion
    }
}
