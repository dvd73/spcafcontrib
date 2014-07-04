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
     CheckId = CheckIDs.Rules.ListTemplate.AddClientSideTemplatesForSPView,
     DisplayName = "Add js client side templates for SPView",
     Message = "Add js client side templates for SPView [{0}].",
     Description = "Add js client side templates for SPView to enable rich experience; quick search, drag and drop, etc.",
     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "15" },
     Resolution = "")]
    public class AddClientSideTemplatesForSPView : Rule<ViewDefinition>
    {
        #region methods

        public override void Visit(ViewDefinition target, NotificationCollection notifications)
        {
            // SPCAF lacks JSLink properties yet :(
            //if(target.jsli
        }

        #endregion
    }
}
