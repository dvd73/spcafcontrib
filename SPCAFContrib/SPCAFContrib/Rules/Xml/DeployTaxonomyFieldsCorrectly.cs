using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using System;
using System.Linq;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.FieldTemplate.DeployTaxonomyFieldsCorrectly,
        Help = CheckIDs.Rules.FieldTemplate.DeployTaxonomyFieldsCorrectly_HelpUrl,

        DisplayName = "Deploy Taxonomy fields correctly.",
        Message = "Do not deploy taxonomy field '{0}' in list definition.",
        Description = "Checks Taxonomy fields definitions.",
        Resolution = "Deploy Taxonomy fields correctly.",

        DefaultSeverity = Severity.Warning,
        SharePointVersion = new[] { "14", "15" },

        Links = new []
        {
            "Provisioning SharePoint 2010 Managed Metadata fields",
            "http://www.sharepointconfig.com/2011/03/the-complete-guide-to-provisioning-sharepoint-2010-managed-metadata-fields/",
            "SharePoint 2010 Managed Metadata - Andrew Connell",
            "http://www.andrewconnell.com/blog/SP2010-Managed-Metadata-About-the-series#0jl1VUAR6hRs2oaR.99"
        }
        )]
    public class DeployTaxonomyFieldsCorrectly : Rule<FieldDefinition>
    {
        public override void Visit(FieldDefinition target, NotificationCollection notifications)
        {
            if (string.IsNullOrEmpty(target.Type) || !target.Type.StartsWith("TaxonomyFieldType"))
            {
                return;
            }

            if (target.ShowField != null && target.ShowField != "Term$Resources:core,Language;")
            {
                Notify<FieldDefinition, string>(target, "Remove ShowField attribute", notifications, f => f.ShowField);
            }

            if (target.Type == "TaxonomyFieldTypeMulti" && (!target.MultSpecified || !target.Mult.IsTrue()))
            {
                Notify<FieldDefinition, TRUEFALSE>(target, "Add Multi=\"TRUE\" attribute", notifications, f => f.Mult);
            }

            if (target.Customization != null && target.Customization.ArrayOfProperty != null)
            {
                List<Guid> textFieldIds = (from p1 in target.Customization.ArrayOfProperty
                                    from p2 in p1.Property
                                    let nodes = p2.Any
                                    where nodes.Length == 2
                                    where nodes[0].Name == "Name" && nodes[0].InnerText == "TextField"
                                    where nodes[1].Name == "Value"
                                    select new Guid(nodes[1].InnerText)
                                    ).ToList();

                IEnumerable<FieldDefinition> fields = from field in target.ParentSolution.ChildsOfType<FieldDefinition>()
                             where textFieldIds.Contains(new Guid(field.ID))
                             select field;

                foreach (FieldDefinition textField in fields)
                {
                    string message = string.Format("Do not deploy TextField '{0}' for TaxonomyField '{1}'", textField.Name, target.Name);
                    Notify(target, message, textField.GetSummary(), notifications);
                }
            }
        }
    }
}
