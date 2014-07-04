using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Control
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXPage.AvoidJQueryDocumentReadyInPage,
     Help = CheckIDs.Rules.General.AvoidJQueryDocumentReady_HelpUrl,

     DisplayName = "Avoid using jQuery(document).ready in page.",
     Description = "Due to specific SharePoint client side initialization life cycle, it is recommended to avoid using jQuery(document).ready call.",
     Message = "jQuery(document).ready is used in page [{0}].",
     Resolution = "Use _spBodyOnLoadFunctions.push function or mQuery for SP2013.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" })]
    public class AvoidJQueryDocumentReadyInPage : Rule<ASPXFile>
    {
        #region methods

        public override void Visit(ASPXFile target, NotificationCollection notifications)
        {
            target.FindJScript(true,
                (s) => { return s.FindJQueryDocumentReadyByIndexOf(); },
                (s) => { return s.FindJQueryDocumentReadyByIndexOf(); },
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
