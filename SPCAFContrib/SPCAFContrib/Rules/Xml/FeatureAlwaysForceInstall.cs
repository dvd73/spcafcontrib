using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
      CheckId = CheckIDs.Rules.Feature.FeatureAlwaysForceInstall,
      Help = CheckIDs.Rules.Feature.FeatureAlwaysForceInstall_HelpUrl,

      DisplayName = "Consider to set \"Always force install\" feature flag to True.",
      Message = "Consider to explicitly set \"Always force install\" flag for the feature [{0}] to True.",
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
