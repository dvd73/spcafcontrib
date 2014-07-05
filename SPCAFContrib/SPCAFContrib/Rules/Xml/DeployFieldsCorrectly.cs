using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using System;
using System.Linq;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.FieldTemplate.DeployFieldsCorrectly,
        Help = CheckIDs.Rules.FieldTemplate.DeployFieldsCorrectly_HelpUrl,

        Message = "Deploy fields correctly.",
        DisplayName = "Deploy fields correctly.",
        Description = "Deploy fields correctly.",
        Resolution = "Deploy fields correctly.",

        DefaultSeverity = Severity.Error,
        SharePointVersion = new[] { "14", "15" },
        Links = new []
        {
            "Fields Element (List)",
            "http://msdn.microsoft.com/en-us/library/office/ms451470.aspx"
        }
        )]
    public class DeployFieldsCorrectly : Rule<FieldDefinition>
    {
        private static string[] GlobalListNames = new string[] //From Microsoft.SharePoint.dll
        {
	        "Lists",
	        "Docs",
	        "WebParts",
	        "ComMd",
	        "Webs",
	        "Workflow",
	        "WFTemp",
	        "Solutions",
	        "Self",
	        "UserInfo"
        };
        public override void Visit(FieldDefinition target, NotificationCollection notifications)
        {
            if (target.GetXPath().StartsWith("/ns:List/ns:MetaData"))
            {
                //Skip fields in List Schema
                return;
            }

            if (!string.IsNullOrEmpty(target.Version))
            {
                string message = string.Format("Remove attribute Version from field [{0}]", target.Name);
                Notify(target, message, notifications, t => t.Version);
            }

            if (target.Type == "Lookup" || target.Type == "LookupMulti")
            {
                if (string.IsNullOrWhiteSpace(target.ShowField))
                {
                    string message = string.Format("Add ShowField=\"Title\" attribute to field [{0}]", target.Name);
                    Notify(target, message, notifications);
                }

                if (!string.IsNullOrWhiteSpace(target.WebId) && string.Compare(target.WebId, "~sitecollection", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    string message = string.Format("Set WebId=\"~sitecollection\" or remove WebId attribute from field [{0}]", target.Name);
                    Notify(target, message, notifications, t => t.WebId);

                }

                string list = target.List;
                if (string.IsNullOrWhiteSpace(list))
                {
                    string message = string.Format("Add List=\"{{WebRelativeListUrl}}\" attribute to field [{0}]", target.Name);
                    Notify(target, message, notifications);

                }
                else
                {

                    Guid dummy;
                    if (Guid.TryParse(list, out dummy))
                    {
                        string message = string.Format("Change List attribute from GUID to ListUrl for field [{0}]", target.Name);
                        Notify(target, message, notifications, t => t.List);

                    }
                    else
                    {
                        if (!GlobalListNames.Contains(list, StringComparer.OrdinalIgnoreCase))
                        {
                            FeatureDefinition fieldFeature = target.FindParentFeature();
                            IEnumerable<ListInstanceDefinition> lists = from li in target.ParentSolution.ChildsOfType<ListInstanceDefinition>()
                                        where li.Url.Equals(list, StringComparison.OrdinalIgnoreCase)
                                        select li;

                            string[] listFeatureIds = lists.Select(l => l.ParentFeature.Id).Distinct().ToArray();

                            if (listFeatureIds.Length == 0)
                            {
                                string message = String.Format("List with url [{1}] not found in solution.", target.Name, list);
                                Notify(target, message, notifications, t => t.List);
                            }
                            else if (!(listFeatureIds.Contains(fieldFeature.Id) || fieldFeature.ActivationDependencies.Any(dep => listFeatureIds.Contains(dep.FeatureId))))
                            {
                                string message = string.Format("Deploy list with url [{1}] in same feature with field [{0}] or add activation dependency from feature [{2}] to feature [{3}].", target.Name, list, fieldFeature.Id, listFeatureIds[0]);
                                Notify(target, message, notifications, t => t.List);
                            }

                        }

                    }
                }


            }
        }
    }
}
