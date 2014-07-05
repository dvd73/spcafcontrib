using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
       CheckId = CheckIDs.Rules.ListTemplate.DoNotAllowDeletionForList,
       Help = CheckIDs.Rules.ListTemplate.DoNotAllowDeletionForList_HelpUrl,

       Message = "Add attribute AllowDeletion=\"FALSE\" to the [{0}] list template.",
       DisplayName = "Prevent SharePoint list from deletion.",
       Description = "If we don't want to let the users to delete the configuration list in SharePoint provide template with attribute AllowDeletion=\"FALSE\".",
       Resolution = "Add attribute AllowDeletion=\"FALSE\" to the list template.",

       DefaultSeverity = Severity.Information,
       SharePointVersion = new[] { "12", "14", "15" },
       Links = new[] 
       { 
           "ListTemplate Element (List Template)",
           "http://msdn.microsoft.com/en-us/library/office/ms462947.aspx" 
       })]
    public class DoNotAllowDeletionForList : Rule<ListTemplateDefinition>
    {
        public override void Visit(ListTemplateDefinition target, NotificationCollection notifications)
        {
            if (!target.AllowDeletionSpecified)
                Notify(target, String.Format(this.MessageTemplate(), target.Name), notifications);
        }
    }
}
