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
     CheckId = CheckIDs.Rules.ASCXFile.AvoidJQueryDocumentReadyInControl,
     Help = CheckIDs.Rules.General.AvoidJQueryDocumentReady_HelpUrl,

     Message = "jQuery(document).ready is used in control [{0}].",
     DisplayName = "Avoid using jQuery(document).ready in ASCX user control.",
     Description = "Due to specific SharePoint client side initialization life cycle, it is recommended to avoid using jQuery(document).ready call.",
     Resolution = "Use _spBodyOnLoadFunctions.push function or SP.SOD.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" })]
    public class AvoidJQueryDocumentReadyInControl : Rule<ASCXFile>
    {
        #region methods

        public override void Visit(ASCXFile target, NotificationCollection notifications)
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
