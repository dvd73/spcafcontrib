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
        CheckId = CheckIDs.Rules.WebPart.WebPartDefinitionMightBeImproved,
        Help = CheckIDs.Rules.WebPart.WebPartDefinitionMightBeImproved_HelpUrl,

        Message = "Web part description might be improved.",
        DisplayName = "Web part description might be improved.",
        Description = "Web part description might be improved.",
        Resolution = "Review and improve default web part descriptions.",

        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] { "12", "14", "15" },
        Links = new []
        {
            "Creating Web Parts for SharePoint",
            "http://msdn.microsoft.com/en-us/library/ee231579.aspx"
        }
        )]
    public class WebPartDefinitionMightBeImproved : Rule<WebPartFile>
    {
        #region properties

        private const string CatalogIconImageUrlPropertyName = "CatalogIconImageUrl";
        private const string TitleIconImageUrlPropertyName = "TitleIconImageUrl";

        private readonly List<string> RestrictedDescriptions = new List<string>(new[] { "My Web Part", "My Visual Web Part" });

        private string _restrictedDescriptionsString;

        private string RestrictedDescriptionsString
        {
            get
            {
                if (string.IsNullOrEmpty(_restrictedDescriptionsString))
                    _restrictedDescriptionsString = string.Join(" , ", RestrictedDescriptions.ToArray());

                return _restrictedDescriptionsString;
            }
        }

        #endregion

        #region methods

        public override void Visit(WebPartFile target, NotificationCollection notifications)
        {
            CheckWebPartProperty(target, CatalogIconImageUrlPropertyName, notifications);
            CheckWebPartProperty(target, TitleIconImageUrlPropertyName, notifications);
            CheckWebPartDescriptionProperty(target, notifications);
        }

        #region checks

        private void CheckWebPartDescriptionProperty(WebPartFile target, NotificationCollection notifications)
        {
            if (string.IsNullOrEmpty(target.Description))
                Notify(target, string.Format("Webpart file [{0}] should have description", target.WebPartFileName), notifications);

            RestrictedDescriptions.ForEach(restrictedDescription =>
            {
                if (string.Compare(restrictedDescription, target.Description, true) == 0)
                {
                    Notify(target, string.Format("Webpart file [{0}] should not have description [{1}]. Current value:[{2}]",
                            target.WebPartFileName,
                            RestrictedDescriptionsString,
                            target.Description), notifications);
                }
            });
        }

        private void CheckWebPartProperty(WebPartFile target, string propertyName, NotificationCollection notifications)
        {
            if (!target.HasProperty(propertyName))
                Notify(target, string.Format("Webpart file [{0}] might have [{1}] property.", target.WebPartFileName, propertyName), notifications);

            string propertyValue = target.GetPropertyValue(propertyName);

            if (string.IsNullOrEmpty(propertyValue))
                Notify(target, string.Format("Webpart file [{0}] might have not null or empty [{1}] property. ", target.WebPartFileName, propertyName), notifications);
        }

        #endregion

        #endregion
    }
}
