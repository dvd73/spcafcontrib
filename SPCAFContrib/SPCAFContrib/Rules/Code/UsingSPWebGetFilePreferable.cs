using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
        CheckId = CheckIDs.Rules.Assembly.UsingSPWebGetFilePreferable,
        Help = CheckIDs.Rules.Assembly.UsingSPWebGetFilePreferable_HelpUrl,

        Message = "Using SPWeb.GetFile method is preferable.",
        DisplayName = "Using SPWeb.GetFile method is preferable.",
        Description = "SPFolder.Files[] will fail with an ArgumentException if the file does not exist.",
        Resolution = "Using SPWeb.GetFile method.",

        DefaultSeverity = Severity.Warning,
        SharePointVersion = new[] { "12", "14", "15" }
        )]
    public class UsingSPWebGetFilePreferable : SearchPropertyRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPFolder, "Files");
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            EnsureTypeMap();

            assembly.OnPropertyUsageMatch(TargetTypeMap, (_instruction) =>
            {
                this.OnMatch(assembly, _instruction, notifications, GetNotificationMessage, () =>
                {
                    return GetSummary(assembly, _instruction);
                });
            });
        }
    }
}
