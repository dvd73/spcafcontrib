using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUsingAjaxControlToolkit,
     Help = CheckIDs.Rules.Assembly.AvoidUsingAjaxControlToolkit_HelpUrl,

     Message = "Avoid AjaxControlToolkit utilization.",
     DisplayName = "Avoid AjaxControlToolkit utilization.",
     Description = "AjaxControlToolkit might work not well with SharePoint controls/pages, requires web config modification, causes issues with postback/update panels.",
     Resolution = "Consider utilizing client side technologies such as JavaScript/jQuery. ",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" },

     Links = new[]
     {
        "Using AJAX Capabilities with a Web Part",
        "http://msdn.microsoft.com/en-us/library/ff648708.aspx"
     }
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
