using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
      CheckId = CheckIDs.Rules.Feature.FeatureAlwaysForceInstall,
      Help = CheckIDs.Rules.Feature.FeatureAlwaysForceInstall_HelpUrl,

      Message = "Recommended to explicitly set \"Always force install\" flag for the feature [{0}] to True.",
      DisplayName = "Recommended to set \"Always force install\" feature flag to True.",
      Description = "Some time you can get problem when deploy SharePoint solution via Visual Studio: Error occurred in deployment step 'Add Solution': A feature with ID {Guid} has already been installed in this farm. Use the force attribute to explicitly re-install the feature.",
      Resolution = "It might be recommended to have \"Always force install\" flag for the features to be always True.",

      DefaultSeverity = Severity.Warning,
      SharePointVersion = new[] { "12", "14", "15" },
      Links = new[]
      {
          "Feature Element (Feature)",
          "http://msdn.microsoft.com/en-us/library/office/ms436075.aspx"
      })]
    public class FeatureAlwaysForceInstall : Rule<FeatureDefinition>
    {
        public override void Visit(FeatureDefinition target, NotificationCollection notifications)
        {
            if (!target.AlwaysForceInstallSpecified)
                Notify(target, String.Format(this.MessageTemplate(), target.FeatureName), notifications);
        }
    }
}
