using System;
using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
    CheckId = CheckIDs.Rules.Assembly.AvoidObsoleteWebServicesInCode,
    DisplayName = "AvoidObsoleteWebServicesInCode",
    Description = "AvoidObsoleteWebServicesInCode",
    DefaultSeverity = Severity.Information,
    SharePointVersion = new[] { "15" },
    Message = "AvoidObsoleteWebServicesInCode",
    Resolution = "AvoidObsoleteWebServicesInCode")]
    public class AvoidObsoleteWebServicesInCode : Rule<AssemblyFileReference>
    {
        #region properties

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            Action<Mono.Cecil.MethodDefinition, Instruction> methodStringCallback = (method, instruction) =>
            {
                string value = instruction.Operand as string;

                if (!string.IsNullOrEmpty(value))
                {
                    SP2007WebServices.AsmxServices.ForEach(asmxService =>
                    {
                        if (value.IndexOf(asmxService, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Notify(assembly,
                               string.Format("Avoid referenrring obsolete asmx services in code. Value:[{0}]", value),
                               new CodeInstruction(method, instruction).ImproveSummary(assembly.GetSummary()),
                               notifications);
                        }
                    });

                    SP2010WebServices.SvcServices.ForEach(svcService =>
                    {
                        if (value.IndexOf(svcService, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Notify(assembly,
                               string.Format("Avoid referenrring obsolete svc services in code. Value:[{0}]", value),
                               new CodeInstruction(method, instruction).ImproveSummary(assembly.GetSummary()),
                               notifications);
                        }
                    });
                }
            };

            CachedOperationExtensions.SearchMethodStringInstructions(assembly, methodStringCallback);
        }

        #endregion
    }
}
