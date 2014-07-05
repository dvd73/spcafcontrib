using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Control
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.ASCXFile.AvoidUsingRenderingTemplate,
     Help = CheckIDs.Rules.ASCXFile.AvoidUsingRenderingTemplate_HelpUrl,

     Message = "RenderingTemplate is used in the user contol [{0}].",
     DisplayName = "RenderingTemplate is used.",
     Description = "Using RenderingTemplate is a too strong list form customization.",
     Resolution = "In many cases to change list form UI the DataFormWebPart (instead of ListFormWebPart) + JavaScript are enough.",

     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "Web Parts for views and forms in SharePoint Designer 2010",
         "http://office.microsoft.com/en-us/sharepoint-designer-help/web-parts-for-views-and-forms-in-sharepoint-designer-2010-HA101805424.aspx#_Toc245608912",
         "ListFieldIterator class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.webcontrols.listfielditerator.aspx",
         "RenderingTemplate class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.webcontrols.renderingtemplate.aspx"
     }
     )]
    public class AvoidUsingRenderingTemplate : Rule<ASCXFile>
    {
        #region methods

        public override void Visit(ASCXFile target, NotificationCollection notifications)
        {
            target.FindControl("RenderingTemplate", (controlDeclaration, lineNumber, linePosition) =>
            {
                Notify(target, String.Format(this.MessageTemplate(), target.ReadableElementName),
                    target.GetSummaryWithLineInfo(lineNumber, linePosition),
                    notifications);
            });
        }

        #endregion
    }
}
