using System.Collections.Generic;
using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
      CheckId = CheckIDs.Rules.FieldTemplate.DoNotDefineMultipleFieldGroupInOneElementFile,
      Help = CheckIDs.Rules.FieldTemplate.DoNotDefineMultipleFieldGroupInOneElementFile_HelpUrl,

      Message = "Do not define multiple field groups in one element file.",
      DisplayName = "Do not define multiple field groups in one element file.",
      Description = "Do not define multiple field groups in one element file.",
      Resolution = "Split up field definitions into different elements.xml per every field group.",

      DefaultSeverity = Severity.Information,
      SharePointVersion = new[] { "12", "14", "15" }
      )]
    public class DoNotDefineMultipleFieldGroupInOneElementFile : Rule<ElementDefinitionCollection>
    {
        #region methods

        public override void Visit(ElementDefinitionCollection target, NotificationCollection notifications)
        {
            if (target == null)
                return;

            if (target.Items == null)
                return;

            IEnumerable<FieldDefinition> fields = target.Items
                               .OfType<FieldDefinition>()
                               .Where(f => f.Parent is ElementManifestReference);

            // probably, the best 'key' for our task?
            IEnumerable<IGrouping<string, FieldDefinition>> fieldFileGroups = fields.GroupBy(field => ((ElementManifestReference)field.Parent).HiveRelativeFileName);

            foreach (IGrouping<string, FieldDefinition> fieldGroup in fieldFileGroups)
            {
                IEnumerable<string> groupNames = fieldGroup
                                    .Where(field => !string.IsNullOrEmpty(field.Group) && !string.IsNullOrEmpty(field.Group.Trim()))
                                    .Select(field => field.Group)
                                    .Distinct();

                if (groupNames.Count() > 1)
                {
                    FieldDefinition firstContentType = fieldGroup.FirstOrDefault();
                    string printableGroupString = groupNames.Aggregate((a, b) => string.Format("[{0}], [{1}]", a, b));

                    Notify(firstContentType,
                           string.Format("Avoid multiple field groups inside element file. Field groups detected: {1}.",
                                fieldGroup.Key, printableGroupString),
                           notifications);
                }
            }
        }

        #endregion
    }
}
