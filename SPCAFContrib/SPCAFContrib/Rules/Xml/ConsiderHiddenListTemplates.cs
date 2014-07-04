using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using System;
using System.Linq;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
       CheckId = CheckIDs.Rules.ListTemplate.ConsiderHiddenListTemplates,
       Help = CheckIDs.Rules.ListTemplate.ConsiderHiddenListTemplates_HelpUrl,

       DisplayName = "Consider create hidden list templates.",
       Message = "Consider making List Template '{0}' hidden.",
       Description = "List Templates with instances in solution should not be created by end user.",
       Resolution = @"Declare Hidden=""TRUE"".",

       DefaultSeverity = Severity.Warning,
       SharePointVersion = new[] { "12", "14", "15" },
       Links = new []
       {
           "ListTemplate Element (List Template)",
           "http://msdn.microsoft.com/en-us/library/office/ms462947.aspx"
       }
       )]
    public class ConsiderHiddenListTemplates : Rule<ListTemplateDefinition>
    {
        public override void Visit(ListTemplateDefinition target, NotificationCollection notifications)
        {
            if (target == null)
            {
                return;
            }

            if (!target.HiddenSpecified || target.Hidden.IsFalse())
            {
                Guid featureId = new Guid(target.FindParentFeature().Id);

                IEnumerable<ListInstanceDefinition> lists = from list in target.ParentSolution.ChildsOfType <ListInstanceDefinition>()
                            where list.TemplateTypeSpecified && list.TemplateType == target.Type
                            where new Guid(list.FeatureId ?? list.ParentFeature.Id) == featureId
                            select list;

                if (lists.Any())
                {
                    Notify(target, string.Format(this.MessageTemplate(), target.Name), notifications, t => t.Hidden);
                }
            }
        }
    }
}
