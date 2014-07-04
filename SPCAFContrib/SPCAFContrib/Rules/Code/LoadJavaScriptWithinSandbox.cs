using System;
using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribSandboxedCompatibilityGroup),
        CheckId = CheckIDs.Rules.Assembly.LoadJavaScriptWithinSandbox,
        Help = CheckIDs.Rules.Assembly.LoadJavaScriptWithinSandbox_HelpUrl,
        DisplayName = "Avoid using ClientScriptManager/ScriptManager.",
        Description = "There is no access to the ClientScriptManager/ScriptManager from within Sandbox.",
        DefaultSeverity = Severity.Information,
        SharePointVersion = new[] {"14", "15"},
        Message = "Avoid using [{0}].",
        Resolution = "Read Waldek Mastykarz article \"Dynamically loading JavaScript from within Sandbox\"",
        Links = new[]
        {
            "Dynamically loading JavaScript from within Sandbox",
            "http://blog.mastykarz.nl/dynamically-loading-javascript-sandbox/"
        })]
    public class LoadJavaScriptWithinSandbox : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            // The ScriptManager is meant to be used with async postbacks, which is why it works with the UpdatePanel. 
            // The ClientScript class is for synchronous postbacks.
            TargetTypeMap.Add(TypeKeys.ScriptManager,
                new List<string>()
                {
                    "RegisterClientScriptBlock",
                    "RegisterClientScriptResource",
                    "RegisterOnSubmitStatement",
                    "RegisterStartupScript",
                    "RegisterNamedClientScriptResource",
                    "RegisterClientScriptResource"
                });
            TargetTypeMap.Add(TypeKeys.ClientScriptManager,
                new List<string>()
                {
                    "RegisterClientScriptBlock",
                    "RegisterClientScriptResource",
                    "RegisterOnSubmitStatement",
                    "RegisterStartupScript",
                    "GetWebResourceUrl"
                });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction,
            NotificationCollection notifications,
            Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            MethodReference md = instruction.Instruction.Operand as MethodReference;
            if (md != null)
            {
                base.OnMatch(assembly, instruction, notifications, ()=>
                {
                    return String.Format(this.MessageTemplate(), md.DeclaringType.FullName);
                }
            , getSummary);
            }
        }

        #endregion
    }
}
