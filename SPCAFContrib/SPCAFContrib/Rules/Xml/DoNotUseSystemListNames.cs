using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using System;
using System.Linq;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
       CheckId = CheckIDs.Rules.ListInstance.DoNotUseSystemListNames,
       Help = CheckIDs.Rules.ListInstance.DoNotUseSystemListNames_HelpUrl,

       DisplayName = "Avoid system list names for the custom list instances.",
       Message = "Avoid system list names for the custom list instances. Tittle: {0}, Url: {1}.",
       Description = "Avoid system list names for the custom list instances.",
       Resolution = @"Do not use out of the box list names.",

       DefaultSeverity = Severity.CriticalWarning,
       SharePointVersion = new[] { "12", "14", "15" },

       Links = new[]
       {
           "List Instances",
           "http://msdn.microsoft.com/en-us/library/office/ms478860.aspx"
       }
       )]
    public class DoNotUseSystemListNames : Rule<ListInstanceDefinition>
    {
        public override void Visit(ListInstanceDefinition target, NotificationCollection notifications)
        {
            if (CheckListTitleAndUrl(target.Title, target.Url))
            {
                Notify(target, String.Format(this.MessageTemplate(), target.Title, target.Url), notifications);
            }
        }

        private bool CheckListTitleAndUrl(string title, string url)
        {
            return SharePointOOBListInstances.ListInstances.Exists(z=>z.Url.Equals(url));
        }
    }
}
