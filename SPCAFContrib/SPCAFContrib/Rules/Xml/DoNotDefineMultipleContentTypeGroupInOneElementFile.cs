using System.Collections.Generic;
using System.Linq;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;

namespace SPCAFContrib.Rules.Xml
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
      CheckId = CheckIDs.Rules.ContentType.DoNotDefineMultipleContentTypeGroupInOneElementFile,
      Help = CheckIDs.Rules.ContentType.DoNotDefineMultipleContentTypeGroupInOneElementFile_HelpUrl,

      DisplayName = "Do not define multiple content type groups in one element file.",
      Message = "Do not define multiple content type groups in one element file.",
      Description = "Do not define multiple content type groups in one element file.",
      Resolution = "Split up content type definitions into different elements.xml per every content type group.",

      DefaultSeverity = Severity.Warning,
      SharePointVersion = new[] { "12", "14", "15" },
      Links=new []
      {
          "Content Type Definitions",
          "http://msdn.microsoft.com/en-us/library/office/ms463449.aspx"
      }
      )]
    public class DoNotDefineMultipleContentTypeGroupInOneElementFile : Rule<ElementDefinitionCollection>
    {
        #region methods

        public override void Visit(ElementDefinitionCollection target, NotificationCollection notifications)
        {
            if (target == null)
                return;

            if (target.Items == null)
                return;

            IEnumerable<ContentTypeDefinition> contentTypes = target.Items
                                     .OfType<ContentTypeDefinition>()
                                     .Where(ct => ct.Parent is ElementManifestReference);

            // probably, the best 'key' for our task?
            IEnumerable<IGrouping<string, ContentTypeDefinition>> contentTypeFileGroups = contentTypes.GroupBy(ct => ((ElementManifestReference)ct.Parent).HiveRelativeFileName);

            foreach (IGrouping<string, ContentTypeDefinition> contentTypeFileGroup in contentTypeFileGroups)
            {
                IEnumerable<string> groupNames = contentTypeFileGroup
                                    .Where(ct => !string.IsNullOrEmpty(ct.Group) && !string.IsNullOrEmpty(ct.Group.Trim()))
                                    .Select(ct => ct.Group).Distinct();

                if (groupNames.Count() > 1)
                {
                    ContentTypeDefinition firstContentType = contentTypeFileGroup.FirstOrDefault();
                    string printableGroupString = groupNames.Aggregate((a, b) => string.Format("[{0}], [{1}]", a, b));

                    Notify(firstContentType,
                           string.Format("Avoid multiple content type groups inside one element file. Content type groups detected:{1}.",
                                contentTypeFileGroup.Key, printableGroupString),
                           notifications);
                }
            }
        }

        #endregion
    }

}
