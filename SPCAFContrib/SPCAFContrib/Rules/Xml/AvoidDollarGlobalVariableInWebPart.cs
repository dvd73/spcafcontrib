using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.WebPart.AvoidDollarGlobalVariableInWebPart,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,
     DisplayName = "Avoid using $ for jQuery in web part.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "14", "15" },
     Message = "jQuery $ variable is used in the web part definition of file [{0}].",
     Resolution = "Use jQuery global variable instead of $.")]
    public class AvoidDollarGlobalVariableInWebPart : Rule<WebPartDefinition>
    {
        #region methods

        public override void Visit(WebPartDefinition target, NotificationCollection notifications)
        {
            if (target.Parent != null && target.Parent.Parent != null
                && target.TypeName!=null && target.TypeName.Equals(TypeKeys.ContentEditorWebPart))
            {
                string[] lines = target.Text;
                int lineNumber = lines.FindJScript((s) => { return s.FindJQueryVariableByIndexOf(); });
                if (lineNumber >= 0)
                {
                    Notify(target,
                        string.Format(this.MessageTemplate(), target.Parent.ReadableElementName),
                        target.GetSummary("Content"), notifications);
                }
            }
        }

        #endregion
    }
}
