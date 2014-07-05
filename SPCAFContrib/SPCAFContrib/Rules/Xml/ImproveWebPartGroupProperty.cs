using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
        CheckId = CheckIDs.Rules.WebPart.WebPartModuleDefinitionMightbeImproved,
        Help = CheckIDs.Rules.WebPart.WebPartModuleDefinitionMightbeImproved_HelpUrl,

        Message = "Web part group property should not be 'Custom'.",
        DisplayName = "Web part group property should not be 'Custom'.",
        Description = "Web part group property should not be 'Custom'.",
        Resolution = "Ensure web part group value has unique name, not 'Custom'.",

        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" },
        Links = new[]
        {
            "Creating Web Parts for SharePoint",
            "http://msdn.microsoft.com/en-us/library/ee231579.aspx"
        }
        )]
    public class WebPartModuleDefinitionMightbeImproved : Rule<ModuleDefinition>
    {
        #region properties

        private readonly List<string> RestrictedGroups = new List<string>(new[] { "Custom" });

        private const bool RequireGroupProperty = true;
        private const string GroupPropertyName = "Group";

        #endregion

        #region methods

        public override void Visit(ModuleDefinition target, NotificationCollection notifications)
        {
            if (!target.ListSpecified || target.List != 113 || target.File == null)
                return;

            foreach (FileDefinition file in target.File)
            {
                if (RequireGroupProperty)
                    CheckPropertyPresence(GroupPropertyName, file, notifications);

                CheckGroupPropertyValue(notifications, file);
            }
        }

        private void CheckGroupPropertyValue(NotificationCollection notifications, FileDefinition file)
        {
            string groupPropertyValue = file.GetPropertyValue(GroupPropertyName);

            RestrictedGroups.ForEach(restrictedGroup =>
            {
                if (string.Compare(groupPropertyValue, restrictedGroup, true) == 0)
                    Notify(file, string.Format("Webpart file [{0}] should not have group called [{1}]", file.Path, restrictedGroup), notifications);
            });
        }

        private void CheckPropertyPresence(string group, FileDefinition file, NotificationCollection notifications)
        {
            if (!file.HasProperty(group) || string.IsNullOrEmpty(file.GetPropertyValue(group)))
                Notify(file, string.Format("Webpart file [{0}] should have [{1}] property.", file.Path, group), notifications);
        }

        #region utils



        #endregion

        #endregion
    }
}
