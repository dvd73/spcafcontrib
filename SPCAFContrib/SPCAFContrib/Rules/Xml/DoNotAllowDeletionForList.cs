using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
       CheckId = CheckIDs.Rules.ListTemplate.DoNotAllowDeletionForList,
       Help = CheckIDs.Rules.ListTemplate.DoNotAllowDeletionForList_HelpUrl,

       DisplayName = "Prevent SharePoint list from deletion.",
       Message = "Add attribute AllowDeletion=\"FALSE\" to the [{0}] list template.",
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
