using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.WebPart.AvoidDollarGlobalVariableInWebPart,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,

     Message = "jQuery $ variable is used in the web part definition of file [{0}].",
     DisplayName = "Avoid using $ as jQuery reference in web part.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Resolution = "Use jQuery global variable instead of $.",
     
     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "14", "15" }
     )]
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
