using System;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk;
using SPCAFContrib.Consts;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
      CheckId = CheckIDs.Rules.Feature.FeatureShouldHaveNotEmptyImageUrl,
      Help = CheckIDs.Rules.Feature.FeatureShouldHaveNotEmptyImageUrl_HelpUrl,

      DisplayName = "Feature should specify ImageUrl attribute.",
      Message = "Feature [{0}] should specify the ImageUrl attribute.",
      Description = "Feature should have not empty ImageUrl attribute and reference a custom feature image.",
      Resolution = "Add custom image to Image folder and reference with ImageUrl attribute.",

      DefaultSeverity = Severity.Warning,
      SharePointVersion = new[] { "12", "14", "15" },

      Links = new[]
        {
            "Feature Element (Feature)",
            "http://msdn.microsoft.com/en-us/library/office/ms436075.aspx"
        }
      )]
    public class FeatureShouldHaveNotEmptyImageUrl : Rule<FeatureDefinition>
    {
        #region methods

        public override void Visit(FeatureDefinition target, NotificationCollection notifications)
        {
            if (string.IsNullOrEmpty(target.ImageUrl))
            {
                Notify(target, String.Format(this.MessageTemplate(), target.FeatureName), notifications);
            }
        }

        #endregion
    }
}

