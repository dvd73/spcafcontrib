using System.Linq;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAF.Sdk;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Feature.MagicStringShouldNotBeUsed,
        Help = CheckIDs.Rules.Feature.MagicStringShouldNotBeUsed_HelpUrl,
        DisplayName = "Do not use hardcoded urls, pathes, emails and account names in xml.",
        Description = "Consider a configuration for solution.",
        DefaultSeverity = Severity.Warning,
        SharePointVersion = new[] { "12", "14", "15" },
        Message = "Hardcoded {1} is detected \"{0}\".",
        Resolution = "Consider a configuration for solution.")]
    public class MagicStringShouldNotBeUsed : Rule<FeatureDefinition>
    {
        #region methods

        public override void Visit(FeatureDefinition target, NotificationCollection notifications)
        {
            if(target.Properties == null)
            {
                return;
            }

            var magicStrings = from property in target.Properties
                               let value = property.Value
                               let match = MagicStringsHelper.Match(value)
                               where match != null
                               select new
                               {
                                   Value = value,
                                   Match = match,
                                   Target = property
                               };

            foreach (var item in magicStrings)
            {
                this.Notify(target,
                               string.Format(this.MessageTemplate(), item.Value, item.Match),
                               target.GetSummary("Properties"),
                               notifications);
            }

        }
        #endregion
    }
}
