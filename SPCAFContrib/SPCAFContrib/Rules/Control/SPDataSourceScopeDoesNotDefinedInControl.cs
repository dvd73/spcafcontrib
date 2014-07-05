using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Control
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASCXFile.SPDataSourceScopeDoesNotDefinedInControl,
     Help = CheckIDs.Rules.General.SPDataSourceScopeDoesNotDefined_HelpUrl,

     Message = "SPDataSource scope is not defined in the user contol [{0}].",
     DisplayName = "SPDataSource scope is not defined in user control.",
     Description = "All SPViewScope enumeration values are covered all possible developer's intentions. If not specified SharePoint will use Default value.",
     Resolution = "Specify Scope property for SPDataSource object.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12","14", "15" },
     Links = new[]
     {
         "SPViewScope enumeration",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spviewscope.aspx",
         "SPDataSource.Scope property",
         "http://msdn.microsoft.com/EN-US/library/microsoft.sharepoint.webcontrols.spdatasource.scope.aspx"
     }
     )]
    public class SPDataSourceScopeDoesNotDefinedInControl : Rule<ASCXFile>
    {
        #region methods

        public override void Visit(ASCXFile target, NotificationCollection notifications)
        {
            target.FindControl("SPDataSource", (controlDeclaration, lineNumber, linePosition) =>
            {
                if (controlDeclaration.IndexOf(" Scope=", StringComparison.InvariantCultureIgnoreCase) == -1)
                    Notify(target, String.Format(this.MessageTemplate(), target.ReadableElementName),
                                        target.GetSummaryWithLineInfo(lineNumber, linePosition),
                                        notifications);
            });
        }

        #endregion
    }
}
