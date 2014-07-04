using System;
using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidEnumeratingAllUserProfiles,
     Help = CheckIDs.Rules.Assembly.AvoidEnumeratingAllUserProfiles_HelpUrl,

     DisplayName = "Avoid enumerating all user profiles.",
     Message = "Avoid enumerating all the user profiles.",
     Description = "Some recommended practices regarding UserProfileManager class utilization.",
     Resolution = "Consider utilizing search to get user profiles you are interested in. After that, retrieve  needed user profiles by \"logic\" look up.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "UserProfileManager class",
         "http://msdn.microsoft.com/en-us/library/microsoft.office.server.userprofiles.userprofilemanager.aspx"
     }
     )]
    public class AvoidEnumeratingAllUserProfiles : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.ProfileManagerBase, new List<string>{
                    "GetEnumerator"
                });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction,
            NotificationCollection notifications, Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            if (instruction.MethodDefinition.HasUserProfileEnumeration())
                base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, instruction);
                });
        }

        #endregion
    }
}
