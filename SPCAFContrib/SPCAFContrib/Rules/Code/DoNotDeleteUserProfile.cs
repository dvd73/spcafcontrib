using System;
using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Assembly.DoNotDeleteUserProfile,
        Help = CheckIDs.Rules.Assembly.DoNotDeleteUserProfile_HelpUrl,

        Message = "Do not delete user profile.",
        DisplayName = "Do not delete user profile.",
        Description = "User profile deletion is a important thing in the solution arch and you have to be seriosly motivated to do this. Generraly, SharePoint is synchronized with AD. If so and you delete user profile it would issued to unpredictable SharePoint behavoir.",
        Resolution = "Use custom user profile property to mark as unnesesary for you.",

        DefaultSeverity = Severity.CriticalWarning,
        SharePointVersion = new[] { "12", "14", "15" }
        )]
    public class DoNotDeleteUserProfile : SearchMethodRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.UserProfileManager, "RemoveUserProfile");
        }
    }
}
