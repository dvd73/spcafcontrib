using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.MasterPage
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXMasterPage.AvoidDollarGlobalVariableInMasterPage,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,
     
     DisplayName = "Avoid using $ for jQuery in master page.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Message = "jQuery $ variable is used in master page [{0}].",
     Resolution = "Use jQuery global variable instead of $.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidDollarGlobalVariableInMasterPage : Rule<SolutionDefinition>
    {
        #region methods

        public override void Visit(SolutionDefinition target, NotificationCollection notifications)
        {
            foreach (InternalSourceFile internalSourceFile in target.AllFiles)
            {
                if (internalSourceFile.FileExtension.ToLower().Contains(".master"))
                {
                    internalSourceFile.FindJScript(true,
                        (s) => { return s.FindJQueryVariableByEngine(); },
                        (s) => { return s.FindJQueryVariableByIndexOf(); },
                        (lineNumber, linePosition) =>
                        {
                            Notify(target, String.Format(this.MessageTemplate(), internalSourceFile.ReadableElementName),
                                internalSourceFile.GetSummaryWithLineInfo(lineNumber, linePosition),
                                notifications);
                        });
                }
            }
        }

        #endregion
    }
}
