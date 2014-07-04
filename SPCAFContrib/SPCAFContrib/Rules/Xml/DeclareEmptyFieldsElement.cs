using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using System.Linq;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
      CheckId = CheckIDs.Rules.ListTemplate.DeclareEmptyFieldsElement,
      Help = CheckIDs.Rules.ListTemplate.DeclareEmptyFieldsElement_HelpUrl,

      DisplayName = "Declare empty Fields element.",
      Message = "Declare empty Fields element.",
      Description = "Declare empty Fields element when using ContentTypeRefs. Fields automatically populated from content types.",
      Resolution = "Declare empty Fields element.",

      DefaultSeverity = Severity.Warning,
      SharePointVersion = new[] { "12", "14", "15" },
      Links = new []
      {
          "Fields Element (List)",
          "http://msdn.microsoft.com/en-us/library/office/ms451470.aspx"
      }
      )]
    public class DeclareEmptyFieldsElement : Rule<ListTemplateDefinition>
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

            if (!contentTypes.Any())
            {
                FieldDefinitions fields = target.ListDefinition.MetaData.Fields;

                if (fields == null || (fields.Field != null && fields.Field.Length > 0))
                {
                    this.Notify(target, this.MessageTemplate(), notifications);
                }
            }
        }
    }
}
