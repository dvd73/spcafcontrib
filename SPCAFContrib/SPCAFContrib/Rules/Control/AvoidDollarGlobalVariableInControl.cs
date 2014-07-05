using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Control
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASCXFile.AvoidDollarGlobalVariableInControl,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,

     Message = "jQuery $ variable is used in the user contol [{0}].",
     DisplayName = "Avoid using $ as jQuery reference in ASCX user control.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Resolution = "Use jQuery global variable instead of $.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidDollarGlobalVariableInControl : Rule<ASCXFile>
    {
        #region methods

        public override void Visit(ASCXFile target, NotificationCollection notifications)
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
