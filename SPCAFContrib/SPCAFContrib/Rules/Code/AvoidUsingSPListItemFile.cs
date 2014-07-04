using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUsingSPListItemFile,
     Help = CheckIDs.Rules.Assembly.AvoidUsingSPListItemFile_HelpUrl,

     Message = "Do not use SPListItem.File.",
     DisplayName = "Do not use SPListItem.File.",
     Description = "For non document library it returns null.",
     Resolution = "Check that you work with document library. Use SPWeb.GetFile(SPListItem.UniqueId) instead.",

     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "SPListItem.File property",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.splistitem.file.aspx"
     }
     )]
    public class AvoidUsingSPListItemFile : SearchPropertyRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPListItem, "File");
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
