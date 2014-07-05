using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
      CheckId = CheckIDs.Rules.ListTemplate.DoNotDeployTaxonomyFieldsInList,
      Help = CheckIDs.Rules.ListTemplate.DoNotDeployTaxonomyFieldsInList_HelpUrl,

      Message = "Do not deploy taxonomy field [{0}] using list definition.",
      DisplayName = "Do not deploy Taxonomy fields using list definitions.",
      Description = "Taxonomy fields should be deployed by content types.",
      Resolution = "Create site column, add it to content type, then add ContentTpeRef to list definition.",

      DefaultSeverity = Severity.CriticalError,
      SharePointVersion = new[] { "14", "15" },
      Links = new []
      {
          "Provisioning SharePoint 2010 Managed Metadata fields",
          "http://www.sharepointconfig.com/2011/03/the-complete-guide-to-provisioning-sharepoint-2010-managed-metadata-fields/"
      }
      )]
    public class DoNotDeployTaxonomyFieldsInListDefinition : Rule<ListTemplateDefinition>
    {
        public override void Visit(ListTemplateDefinition target, NotificationCollection notifications)
        {
            if (target == null
                || target.ListDefinition == null
                || target.ListDefinition.MetaData == null
                || target.ListDefinition.MetaData.Fields == null
                || target.ListDefinition.MetaData.Fields.Field == null)
            {
                return;
            }

            FieldDefinition[] fields = target.ListDefinition.MetaData.Fields.Field;

            foreach (FieldDefinition field in fields)
            {
                if (!string.IsNullOrEmpty(field.Type) && field.Type.StartsWith("TaxonomyFieldType"))
                {
                    string message = string.Format(this.MessageTemplate(), field.Name);
                    this.Notify(field, message, field.GetSummary("Type"), notifications);
                }
            }
        }
    }
}
