using System;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Collectors;
using SPCAFContrib.Common.Statistics;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
     [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUsingListFieldIterator,
     Help = CheckIDs.Rules.Assembly.AvoidUsingListFieldIterator_HelpUrl,

     Message = "ListFieldIterator is used.",
     DisplayName = "ListFieldIterator is used.",
     Description = "Using ListFieldIterator is a too strong list form customization.",
     Resolution = "In many cases to change list form UI the DataFormWebPart (instead of ListFormWebPart) + JavaScript are enough.",

     DefaultSeverity = Severity.Information,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new[]
     {
         "Web Parts for views and forms in SharePoint Designer 2010",
         "http://office.microsoft.com/en-us/sharepoint-designer-help/web-parts-for-views-and-forms-in-sharepoint-designer-2010-HA101805424.aspx#_Toc245608912",
         "ListFieldIterator class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.webcontrols.listfielditerator.aspx",
         "RenderingTemplate class",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.webcontrols.renderingtemplate.aspx"
     }
     )]
    public class AvoidUsingListFieldIterator : SearchMethodRuleBase
    {
         public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
         {
             SolutionDefinition solution = assembly.ParentSolution as SolutionDefinition;

             bool hasSharePointReference = assembly.AssemblyReferencesSharePointAssembly();
             if (!hasSharePointReference) return;

             if (assembly.AssemblyHasExcluded()) return;

             if (!IteratorStatistic.Instance.Collected)
             {
                 new IteratorCollector().Visit(solution);
             }

             if (IteratorStatistic.Instance.Collected)
             {
                 TypeDefinition customIterator = assembly.AllTypeDefinitions().FirstOrDefault(td => IteratorStatistic.Instance.CustomTypes.Contains(td.FullName));

                 if (customIterator != null)
                    Notify(assembly, this.MessageTemplate(), customIterator.ImproveSummary(assembly.GetSummary()), notifications);
             }
         }
         

         protected override void PopulateTypeMap()
         {
             throw new NotImplementedException();
         }
    }
}
