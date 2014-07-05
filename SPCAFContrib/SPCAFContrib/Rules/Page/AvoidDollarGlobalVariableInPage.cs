using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Page
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXPage.AvoidDollarGlobalVariableInPage,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,

     Message = "jQuery $ variable is used in the page [{0}].",
     DisplayName = "Avoid using $ as jQuery reference in ASPX page.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Resolution = "Use jQuery global variable instead of $.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidDollarGlobalVariableInPage: Rule<ASPXFile>
    {
        #region methods

        public override void Visit(ASPXFile target, NotificationCollection notifications)
        {
            target.FindJScript(true,
                (s) => { return s.FindJQueryVariableByEngine(); },
                (s) => { return s.FindJQueryVariableByIndexOf(); },
                (lineNumber, linePosition) =>
                {
                    Notify(target, String.Format(this.MessageTemplate(), target.ReadableElementName),
                        target.GetSummaryWithLineInfo(lineNumber, linePosition),
                        notifications);
                });
        }

        #endregion
    }
}
