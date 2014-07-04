using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Rules.Xml
{
     [RuleMetadata(typeof(ContribBestPracticesGroup),
       CheckId = CheckIDs.Rules.FieldTemplate.DoNotAllowDeletionForField,
       Help = CheckIDs.Rules.FieldTemplate.DoNotAllowDeletionForField_HelpUrl,

       DisplayName = "Prevent SharePoint field from deletion.",
       Message = "Add attribute AllowDeletion=\"FALSE\" to the [{0}] field template.",
       Description = "If we don't want to let the users to delete the field in SharePoint provide template with attribute AllowDeletion=\"FALSE\".",
       Resolution = "Add attribute AllowDeletion=\"FALSE\" to the field template.",

       DefaultSeverity = Severity.Information,
       SharePointVersion = new[] { "12", "14", "15" },

       Links = new[] 
       { 
           "Field Element (Field)",
           "http://msdn.microsoft.com/en-us/library/office/aa979575.aspx" 
       })]
    public class DoNotAllowDeletionForField : Rule<FieldDefinition>
    {
         public override void Visit(FieldDefinition target, NotificationCollection notifications)
        {
            if (!target.AllowDeletionSpecified)
                Notify(target, String.Format(this.MessageTemplate(), target.Name), notifications);
        }
    }
}
