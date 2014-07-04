using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Model.Extensions;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
    CheckId = CheckIDs.Rules.Assembly.AvoidStaticSPObjectsInFields,
    Help = CheckIDs.Rules.Assembly.AvoidStaticSPObjectsInFields_HelpUrl,
    DisplayName = "Avoid using static SP-Objects In Fields.",
    Description = "Having static SP-Objects as a fields are quite dangerous.",
    DefaultSeverity = Severity.Warning,
    SharePointVersion = new[] { "12", "14", "15" },
    Message = "Avoid using static SP-Objects in fields in class: {0}.",
    Resolution = "Pass SP-Objects as a methods parameters most of the time.")]
    public class AvoidStaicSPObjectsInFields : Rule<AssemblyFileReference>
    {
        #region fields

        private static readonly HashSet<string> _unsafeObjects = new HashSet<string>() { "SPWeb", "SPSite", "SPList", "SPDocumentLibrary", "SPFolder", "SPListItem", "SPFile" };

        #endregion

        #region methods

        public override void Visit(AssemblyFileReference assembly, NotificationCollection notifications)
        {
            if (assembly.AssemblyDefinition == null)
                assembly.AssemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.AssemblyLocation);

            if (assembly.AssemblyHasExcluded()) return;

            IEnumerable<TypeDefinition> types = assembly.AllTypeDefinitions().Where(z => z.IsClass && z.HasFields);

            foreach (TypeDefinition type in types)
            {
                foreach (Mono.Cecil.FieldDefinition field in type.Fields)
                {
                    if (field.IsStatic && !IsSafeType(field.FieldType))
                        Notify(assembly, string.Format(this.MessageTemplate(), field.DeclaringType.Name), field.ImproveSummary(assembly.GetSummary()), notifications);
                }
            }
        }

        private bool IsSafeType(TypeReference type)
        {
            return !_unsafeObjects.Contains(type.Name);
        }


        #endregion
    }
}
