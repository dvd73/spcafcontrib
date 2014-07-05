using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Control
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.WebPart.AvoidJQueryDocumentReadyInWebPart,
     Help = CheckIDs.Rules.General.AvoidJQueryDocumentReady_HelpUrl,

     Message = "jQuery(document).ready is used in the web part definition of file [{0}].",
     DisplayName = "Avoid using jQuery(document).ready in web part.",
     Description = "Due to specific SharePoint client side initialization life cycle, it is recommended to avoid using jQuery(document).ready call.",
     Resolution = "Use _spBodyOnLoadFunctions.push function or SP.SOD.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" })]
    public class AvoidJQueryDocumentReadyInWebPart : Rule<WebPartDefinition>
    {
        public override void Visit(WebPartDefinition target, NotificationCollection notifications)
        {
            if (target.Parent != null && target.Parent.Parent != null
                && target.TypeName != null && target.TypeName.Equals(TypeKeys.ContentEditorWebPart))
            {
                string[] lines = target.Text;
                int lineNumber = lines.FindJScript((s) => { return s.FindJQueryDocumentReadyByIndexOf(); });
                if (lineNumber >= 0)
                {
                    Notify(target,
                        string.Format(this.MessageTemplate(), target.Parent.ReadableElementName),
                        target.GetSummary("Content"), notifications);
                }
            }
        }
    }
}
