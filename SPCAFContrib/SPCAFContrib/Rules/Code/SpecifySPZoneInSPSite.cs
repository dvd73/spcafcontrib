using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Rules.Code.Base;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using ParameterDefinition = Mono.Cecil.ParameterDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.SpecifySPZoneInSPSite,
     Help = CheckIDs.Rules.Assembly.SpecifySPZoneInSPSite_HelpUrl,
     DisplayName = "Missing SPUrlZone parameter in SPSite constructor.",
     Description = "Constructor would take default SPUrlZone so that you may have issues with the *.Url properties.",
     DefaultSeverity = Severity.CriticalWarning,
     SharePointVersion = new[] { "12", "14", "15" },
     Message = "Missing SPUrlZone parameter in SPSite constructor.",
     Links = new[]
     {
         "SPSite constructor",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spsite.spsite.aspx"
     },
     Resolution = "You have to specify SPUrlZone while creating new instance of the SPSite via new SPSite(Guid, ...) constructor. Constructor would take default SPUrlZone so that you may have issues with the *.Url properties.")]
    public class SpecifySPZoneInSPSite : SearchMethodRuleBase
    {
        #region methods

        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPSite, new List<string>{
                    ".ctor"
                });
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
            Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            MethodReference mr = instruction.Instruction.Operand as MethodReference;
            if (mr != null && mr.HasParameters)
            {
                ParameterDefinition p1 = mr.Parameters[0];
                ParameterDefinition p2 = null;
                if (mr.Parameters.Count > 1)                
                    p2 = mr.Parameters[1];
                if (p1.ParameterType.FullName == "System.Guid" && 
                    (mr.Parameters.Count == 1 || (p2 != null && p2.ParameterType.FullName != "Microsoft.SharePoint.Administration.SPUrlZone")))
                    base.OnMatch(assembly, instruction, notifications, getNotificationMessage, getSummary);
            }
        }

        #endregion
    }
}
