using System;
using System.Collections.Concurrent;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
      CheckId = CheckIDs.Rules.ListInstance.UniqueListInstanceUrl,
      Help = CheckIDs.Rules.ListInstance.UniqueListInstanceUrl_HelpUrl,

      Message = "List instance [{0}] has no unique URL: {2}. Duplicated with list [{1}].",
      DisplayName = "List instance URL has to be unique.",
      Description = "Not unique list instance URL might lead to the provision fail.",
      Resolution = "Correct list instance urls in manifest.",

      DefaultSeverity = Severity.Error,
      SharePointVersion = new[] { "12", "14", "15" },
      Links = new []
      {
          "List Instances",
          "http://msdn.microsoft.com/en-us/library/office/ms478860.aspx"
      }
      )]
    public class UniqueListInstanceUrl : Rule<ListInstanceDefinition>
    {

        #region properties

        private static ConcurrentDictionary<string, string> listUrls = new ConcurrentDictionary<string, string>();

        #endregion

        public override void Visit(ListInstanceDefinition listInstance, NotificationCollection notifications)
        {
            while(true)
            {
                string title;
                if (listUrls.TryGetValue(listInstance.Url, out title)) 
                {
                    if (title != listInstance.Title)
                    {
                        Notify(listInstance,
                            String.Format(this.MessageTemplate(), listInstance.Title, listUrls[listInstance.Url], listInstance.Url),
                            notifications);
                    }
                    break;
                }
                else
                {
                    if (listUrls.TryAdd(listInstance.Url, listInstance.Title))
                    {
                        break;                        
                    }
                }
            }
        }
    }
}
