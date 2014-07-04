﻿using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Page
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXPage.SPDataSourceScopeDoesNotDefinedInPage,
     Help = CheckIDs.Rules.General.SPDataSourceScopeDoesNotDefined_HelpUrl,

     DisplayName = "SPDataSource scope is not defined in page.",
     Message = "SPDataSource scope is not defined in page.",
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
    public class SPDataSourceScopeDoesNotDefinedInPage : Rule<ASPXFile>
    {
        #region methods

        public override void Visit(ASPXFile target, NotificationCollection notifications)
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
