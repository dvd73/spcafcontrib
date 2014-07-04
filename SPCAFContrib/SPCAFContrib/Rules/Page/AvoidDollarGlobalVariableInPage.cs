using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Page
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXPage.AvoidDollarGlobalVariableInPage,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,
     
     DisplayName = "Avoid using $ for jQuery in ASPX page.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Message = "jQuery $ variable is used in page [{0}].",
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
