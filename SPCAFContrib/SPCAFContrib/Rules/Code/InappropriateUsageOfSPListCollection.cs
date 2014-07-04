using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SPCAF.Sdk;
using SPCAF.Sdk.Rules;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAFContrib.Common;
using SPCAFContrib.Consts;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;
using SPCAFContrib.Rules.Code.Base;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
     CheckId = CheckIDs.Rules.Assembly.InappropriateUsageOfSPListCollection,
     Help = CheckIDs.Rules.Assembly.InappropriateUsageOfSPListCollection_HelpUrl,

     DisplayName = "Inappropriate SPList collection usage.",
     Message = "Inappropriate SPList collection usage.",
     Description = "Avoid enumeration, LINQ enumeration or string based index call on SPListItem collection.",
     Resolution = "Consider fetching list instance with SPWeb.GetList(listUrl) method",

     DefaultSeverity = Severity.Error,
     SharePointVersion = new[] { "12", "14", "15" },
     Links = new []
     {
         "SPWeb.GetList method",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spweb.getlist.aspx",
         "SPUrlUtility.CombineUrl method",
         "http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.utilities.spurlutility.combineurl.aspx"
     }
     )]
    public class InappropriateUsageOfSPListCollection : Rule<AssemblyFileReference>
    {
        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            CheckTryGetListCall(assembly, notifications);

            IEnumerable<MethodDefinition> allMethods = assembly.AllMethodDefinitions();

            foreach (MethodDefinition method in allMethods)
            {
                CheckStringIndexCalls(assembly, method, notifications);
                CheckLinqCalls(assembly, method, notifications);
                CheckEnumeratorCalls(assembly, method, notifications);
            }
        }

        #region checks

        private void CheckTryGetListCall(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            MultiValueDictionary<string, string> methodsMap = new MultiValueDictionary<string, string>() {
                {
                    TypeKeys.SPListCollection, new List<string>{
                        "TryGetList"
                    }
                }
            };

            assembly.OnMethodMatch(methodsMap, (_methodInstruction) =>
            {
                this.Notify(assembly, "Avoid using TryGetList method.", _methodInstruction.ImproveSummary(assembly.GetSummary()), notifications);
            });
        }

        private void CheckEnumeratorCalls(AssemblyFileReference assembly, Mono.Cecil.MethodDefinition method, NotificationCollection notifications)
        {
            foreach (Instruction call in method.GetSPListCollectionEnumeratorCalls())
            {
                Notify(assembly,
                       string.Format("Avoid all list enumerations via enumerator calls"),
                       new CodeInstruction(method, call).ImproveSummary(assembly.GetSummary()),
                       notifications);
            }
        }

        private void CheckLinqCalls(AssemblyFileReference assembly, Mono.Cecil.MethodDefinition method, NotificationCollection notifications)
        {
            foreach (Instruction call in method.GetSPListCollectionEnumeratorLinqCastCall())
            {
                Notify(assembly,
                       string.Format("Avoid all list enumerations via linq Cast<T> expression"),
                       new CodeInstruction(method, call).ImproveSummary(assembly.GetSummary()),
                       notifications);
            }

            foreach (Instruction call in method.GetSPListCollectionEnumeratorLinqOfTypeCall())
            {
                Notify(assembly,
                       string.Format("Avoid all list enumerations via linq OfType<T> expression"),
                       new CodeInstruction(method, call).ImproveSummary(assembly.GetSummary()),
                       notifications);
            }
        }

        private void CheckStringIndexCalls(AssemblyFileReference assembly, Mono.Cecil.MethodDefinition method, NotificationCollection notifications)
        {
            foreach (Instruction call in method.GetSPListCollectionStringIndexCall())
            {
                Notify(assembly,
                       string.Format("Avoid string based index calls to obtain the list"),
                       new CodeInstruction(method, call).ImproveSummary(assembly.GetSummary()),
                       notifications);
            }
        }

        #endregion

        #endregion

    }
}
