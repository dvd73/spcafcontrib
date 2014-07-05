using System;
using System.Collections.Generic;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.WrongSPViewUsage,
     Help = CheckIDs.Rules.Assembly.WrongSPViewUsage_HelpUrl,

     Message = "Multiple SPView instances could not be updated at once.",
     DisplayName = "Multiple SPView instances could not be updated at once.",
     Description = "SPList.DefaultView and SPList.Views[] properties returns a new SPView instance with every call.",
     Resolution = "To handle a single instance you need to retrieve the SPView object and modify it directly.",

     DefaultSeverity = Severity.Error,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "SPView.Update method",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spview.update.aspx"
     })]
    public class WrongSPViewUpdate : SearchMethodRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPView, new List<string>() { "Update" });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
             Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            MethodReference prev_method = instruction.Instruction.Previous.Operand as MethodReference;

            if (prev_method != null &&
                (prev_method.FullName == "Microsoft.SharePoint.SPView Microsoft.SharePoint.SPList::get_DefaultView()" || 
                prev_method.FullName == "Microsoft.SharePoint.SPView Microsoft.SharePoint.SPViewCollection::get_Item(System.String)"))
                    base.OnMatch(assembly, instruction, notifications, getNotificationMessage, getSummary);
        }
    }
}
