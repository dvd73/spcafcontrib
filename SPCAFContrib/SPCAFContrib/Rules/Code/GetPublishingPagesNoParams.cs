using System;
using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.GetPublishingPages,
     Help = CheckIDs.Rules.Assembly.GetPublishingPages_HelpUrl,

     Message = "Avoid enumerating all PublishingPage objects.",
     DisplayName = "Avoid enumerating all PublishingPage objects.",
     Description = "Some recommended practices regarding GetPublishingPages method utilization.",
     Resolution = "It might be better to use GetPublishingPages(SPQuery query) method instead.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "PublishingWeb.GetPublishingPages method ",
         "http://msdn.microsoft.com/en-us/library/ms493244.aspx",
         "PublishingWeb.GetPublishingPages method (String)",
         "http://msdn.microsoft.com/en-us/library/ms559808.aspx",
         "PublishingWeb.GetPublishingPages method (UInt32)",
         "http://msdn.microsoft.com/en-us/library/ms571021.aspx"
     }
     )]
    public class GetPublishingPagesNoParams : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.PublishingWeb, new List<string>{
                    "GetPublishingPages"
                });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction,
            NotificationCollection notifications, Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            MethodReference mReference = instruction.Instruction.Operand as MethodReference;
            if (mReference != null && !mReference.HasParameters)
                base.OnMatch(assembly, instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, instruction);
                });
        }

        #endregion
    }
}
