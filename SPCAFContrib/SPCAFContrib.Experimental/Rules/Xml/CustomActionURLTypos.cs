using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCAFContrib.Experimental.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.CustomAction.CustomActionURLTypos,
     DisplayName = "URL token typos in custom action",
     Message = "URL token typos in custom action [{0}]. Full URL: [{1}]",
     Description = "",
     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },
     Resolution = "")]
    public class CustomActionURLTypos : Rule<CustomActionDefinition>
    {
        #region static

        static CustomActionURLTypos()
        {
            _allowedTockens = new List<string>();

            _allowedTockens.Add("~site");
            _allowedTockens.Add("~sitecollection");
            _allowedTockens.Add("~layouts");
            _allowedTockens.Add("~siteLayouts");
            _allowedTockens.Add("~siteCollectionLayouts");
        }

        #endregion

        #region properties

        private static List<string> _allowedTockens;

        #endregion

        #region methods

        public override void Visit(CustomActionDefinition target, NotificationCollection notifications)
        {
            if (target == null || target.UrlAction == null || string.IsNullOrEmpty(target.UrlAction.Url))
                return;

            if (!target.UrlAction.Url.StartsWith("~"))
                return;

            var hasAllowedTocken = false;
            var currentUrl = target.UrlAction.Url.ToLower();

            foreach (var allowedToken in _allowedTockens)
            {
                if (currentUrl.StartsWith(allowedToken))
                {
                    hasAllowedTocken = true;
                    break;
                }
            }

            if (!hasAllowedTocken)
            {
                Notify(target,
                    string.Format(this.MessageTemplate(), target.Title, target.UrlAction.Url),
                    target.GetSummary(String.Empty), notifications);
            }
        }

        #endregion
    }
}
