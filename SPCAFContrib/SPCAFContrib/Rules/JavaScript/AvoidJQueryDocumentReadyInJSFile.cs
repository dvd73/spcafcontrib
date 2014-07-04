using System;
using Jurassic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.JavaScript
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.JavaScriptFile.AvoidJQueryDocumentReadyInJSFile,
     Help = CheckIDs.Rules.General.AvoidJQueryDocumentReady_HelpUrl,

     DisplayName = "Avoid using jQuery(document).ready in .js file.",
     Description = "Due to specific SharePoint client side initialization life cycle, it is recommended to avoid using jQuery(document).ready call.",
     Message = "jQuery(document).ready is used in file [{0}].",
     Resolution = "Use _spBodyOnLoadFunctions.push function or mQuery for SP2013.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidJQueryDocumentReadyInJSFile : Rule<JavaScriptFile>
    {
        #region methods

        public override void Visit(JavaScriptFile target, NotificationCollection notifications)
        {
            if (target.FileContent.Length > 0)
            {
                target.FindJScript(false,
                    (s) => { return s.FindJQueryDocumentReadyByIndexOf(); },
                    (s) => { return s.FindJQueryDocumentReadyByIndexOf(); },
                    (lineNumber, linePosition) =>
                    {
                        Notify(target, String.Format(this.MessageTemplate(), target.ReadableElementName),
                            target.GetSummaryWithLineInfo(lineNumber, linePosition),
                            notifications);
                    });
            }
        }

        #endregion
    }
}
