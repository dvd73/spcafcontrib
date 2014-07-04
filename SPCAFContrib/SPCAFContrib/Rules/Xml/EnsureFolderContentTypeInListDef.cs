using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using System.Linq;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
      CheckId = CheckIDs.Rules.ListTemplate.EnsureFolderContentTypeInListDefinition,
      Help = CheckIDs.Rules.ListTemplate.EnsureFolderContentTypeInListDefinition_HelpUrl,

      DisplayName = "Ensure Folder ContentTypeRef in list definition.",
      Message = "List Definition '{0}' should include Forlder ContentTypeRef.",
      Description = "List definitions should include Folder ContentTtype to work correctly.",
      Resolution = "Add ContentTpeRef with ID='0x0120.",

      DefaultSeverity = Severity.CriticalWarning,
      SharePointVersion = new[] { "12", "14", "15" },
      Links = new []
      {
          "ContentTypeRef Element (List)",
          "http://msdn.microsoft.com/en-us/library/office/aa543767.aspx"
      }
      )]
    public class EnsureFolderContentTypeInListDefinition : Rule<ListTemplateDefinition>
    {
        public override void Visit(ListTemplateDefinition target, NotificationCollection notifications)
        {
            if (target == null
                || target.ListDefinition == null
                || target.ListDefinition.MetaData == null
                || target.ListDefinition.MetaData.ContentTypes == null
                || target.ListDefinition.MetaData.ContentTypes.Items == null)
            {
                return;
            }

            IEnumerable<ContentTypeReference> refs = target.ListDefinition.MetaData.ContentTypes.Items.OfType<ContentTypeReference>();
            if (!refs.Any(r => r.ID.ToUpper() == "0x0120".ToUpper()))
            {
                string message = string.Format(this.MessageTemplate(), target.Name);
                this.Notify(target, message, target.ListDefinition.GetSummary("MetaData"), notifications);

            }
        }
    }
}
