﻿using System;
using Jurassic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.JavaScript
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.JavaScriptFile.AvoidDollarGlobalVariableInJSFile,
     Help = CheckIDs.Rules.General.AvoidDollarGlobalVariable_HelpUrl,
     
     DisplayName = "Avoid using $ for jQuery in .js file.",
     Description = "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
     Message = "jQuery $ variable is used in file [{0}].",
     Resolution = "Use jQuery global variable instead of $.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] { "12", "14", "15" }
     )]
    public class AvoidDollarGlobalVariableInJSFile : Rule<JavaScriptFile>
    {
        #region methods

        public override void Visit(JavaScriptFile target, NotificationCollection notifications)
        {
            if (target.FileContent.Length > 0)
            {
                target.FindJScript(false,
                    (s) => { return s.FindJQueryVariableByEngine(); },
                    (s) => { return s.FindJQueryVariableByIndexOf(); },
                    (lineNumber, linePosition) =>
                    {
                        Notify(target, String.Format(this.MessageTemplate(), target.ReadableElementName),
                            target.GetSummaryWithLineInfo(lineNumber, linePosition),
                            notifications);
                    });
            }
        }

        #endregion
    }
}