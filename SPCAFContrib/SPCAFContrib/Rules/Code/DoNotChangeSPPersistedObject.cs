using System;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Utilities;
using SPCAFContrib.Collectors;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.DoNotChangeSPPersistedObject,
     Help = CheckIDs.Rules.Assembly.DoNotChangeSPPersistedObject_HelpUrl,

     DisplayName = "Do not change SPPersistedObject in the content web application.",
     Message = "Do not change SPPersistedObject in the content web application.",
     Description = "SharePoint 2010+ has security feature to all objects inheriting from SPPersistedObject. This feature explicitly disallows modification of SPPersistedObject objects from content web applications.",
     Resolution = "Put your logic into the feature with WebApplication or Farm scope.",

     DefaultSeverity = Severity.Error,
     SharePointVersion = new[] { "14", "15" },
     Links = new[]
     {
         "SPPersistedObject class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.administration.sppersistedobject.aspx",
         "kb 2564009",
         "http://support.microsoft.com/kb/2564009/en-us",
         "\"The SPPersistedObject, XXXXXXXXXXX, could not be updated because the current user is not a Farm Administrator\" craziness in Sharepoint 2010",
         "http://unclepaul84.blogspot.ru/2010/06/sppersistedobject-xxxxxxxxxxx-could-not.html"
     })]
    public class DoNotChangeSPPersistedObject : SearchMethodRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.SPPersistedObject, "Update");
            TypeInfo.SPPersistedObjects.Each(s => TargetTypeMap.Add(s, "Update"));
        }

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            SolutionDefinition solution = assembly.ParentSolution as SolutionDefinition;

            bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
            if (!hasSharePointReference) return;

            if (assembly.AssemblyHasExcluded()) return;

            if (!SPPersistedObjectStatistic.Instance.Collected)
            {
                new SPPersistedObjectCollector().Visit(solution);
            }

            if (SPPersistedObjectStatistic.Instance.Collected)
            {
                base.Visit(assembly, notifications);
            }
        }

        protected override void OnMatch(AssemblyFileReference assembly, CodeInstruction instruction, NotificationCollection notifications,
            Func<string> getNotificationMessage, Func<ElementSummary> getSummary)
        {
            TypeDefinition td = instruction.MethodDefinition.DeclaringType;
            Mono.Cecil.MethodDefinition md = instruction.MethodDefinition;
                
            if (
                !SPPersistedObjectStatistic.Instance.AllowedRecivers.Contains(td.FullName) &&
                (
                    !SPPersistedObjectStatistic.Instance.AllowedReciverCalls.ContainsKey(td.FullName) ||
                    !SPPersistedObjectStatistic.Instance.AllowedReciverCalls[td.FullName].Contains(md.Name)
                )
               )
            {
                base.OnMatch(assembly, instruction, notifications, getNotificationMessage, getSummary);
            } 
        }
    }
}
