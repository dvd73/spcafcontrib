using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUsingAjaxControlToolkit,
     Help = CheckIDs.Rules.Assembly.AvoidUsingAjaxControlToolkit_HelpUrl,

     DisplayName = "Avoid AjaxControlToolkit utilization.",
     Message = "Avoid AjaxControlToolkit utilization.",
     Description = "AjaxControlToolkit might work not well with SharePoint controls/pages, requires web config modification, causes issues with postback/update panels.",
     Resolution = "Consider utilizing client side technologies such as JavaScript/jQuery. ",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidUsingAjaxControlToolkit : Rule<SolutionDefinition>
    {
        #region methods

        public override void Visit(SolutionDefinition solution, NotificationCollection notifications)
        {
            bool hasAjaxControlToolkit = solution.Assemblies != null &&
                                        solution.Assemblies.Any(assembly => !string.IsNullOrEmpty(assembly.Location) &&
                                                                assembly.Location.Contains("AjaxControlToolkit.dll"));

            if (hasAjaxControlToolkit)
                Notify(solution, this.MessageTemplate(), notifications);
        }

        #endregion
    }
}
