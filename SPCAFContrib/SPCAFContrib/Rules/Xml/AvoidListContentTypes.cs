using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using System.Linq;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ContentType.AvoidListContentTypes,
     Help = CheckIDs.Rules.ContentType.AvoidListContentTypes_HelpUrl,

     Message = "Change list content type [{0}] to ContentTypeRef.",
     DisplayName = "Avoid list content types.",
     Description = "Avoid using list content type in List Templates use ContentTypeRef instead.",
     Resolution = "Change list content type  to ContentTypeRef.",

     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "Site and List Content Types",
         "http://msdn.microsoft.com/en-us/library/office/ms463016.aspx"
     }
     )]
    public class AvoidListContentTypes : Rule<ListTemplateDefinition>
    {
        public override void Visit(ListTemplateDefinition target, NotificationCollection notifications)
        {
            if (target == null ||
                target.ListDefinition == null ||
                target.ListDefinition.MetaData == null ||
                target.ListDefinition.MetaData.ContentTypes == null ||
                target.ListDefinition.MetaData.ContentTypes.Items == null)
            {
                return;
            }

            IEnumerable<ContentTypeDefinition> contentTypes = target.ListDefinition.MetaData.ContentTypes.Items.OfType<ContentTypeDefinition>();

            foreach (ContentTypeDefinition contentType in contentTypes)
            {
                string message = string.Format(this.MessageTemplate(), contentType.Name);
                Notify(contentType, message, contentType.GetSummary("Name"), notifications);
            }
        }
    }
}
