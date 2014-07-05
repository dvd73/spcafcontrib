using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.MasterPage
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.ASPXMasterPage.AvoidDollarGlobalVariableInMasterPage,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,

     Message = "jQuery $ variable is used in the master page [{0}].",
     DisplayName = "Avoid using $ as jQuery reference in master page.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
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
