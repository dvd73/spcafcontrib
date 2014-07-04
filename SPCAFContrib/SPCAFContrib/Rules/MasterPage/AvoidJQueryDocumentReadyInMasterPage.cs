using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Control
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXMasterPage.AvoidJQueryDocumentReadyInMasterPage,
     Help = CheckIDs.Rules.General.AvoidJQueryDocumentReady_HelpUrl,

     DisplayName = "Avoid using jQuery(document).ready in master page.",
     Description = "Due to specific SharePoint client side initialization life cycle, it is recommended to avoid using jQuery(document).ready call.",
     Message = "jQuery(document).ready is used in master page [{0}].",
     Resolution = "Use _spBodyOnLoadFunctions.push function or mQuery for SP2013.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" })]
    public class AvoidJQueryDocumentReadyInMasterPage : Rule<SolutionDefinition>
    {
        #region methods

        public override void Visit(SolutionDefinition target, NotificationCollection notifications)
        {
            foreach (InternalSourceFile internalSourceFile in target.AllFiles)
            {
                if (internalSourceFile.FileExtension.ToLower().Contains(".master"))
                {
                    internalSourceFile.FindJScript(true,
                        (s) => { return s.FindJQueryDocumentReadyByIndexOf(); },
                        (s) => { return s.FindJQueryDocumentReadyByIndexOf(); },
                        (lineNumber, linePosition) =>
                        {
                            Notify(target, String.Format(this.MessageTemplate(), internalSourceFile.ReadableElementName),
                                internalSourceFile.GetSummaryWithLineInfo(lineNumber, linePosition),
                                notifications);
                        });
                }
            }
        }

        #endregion
    }
}
