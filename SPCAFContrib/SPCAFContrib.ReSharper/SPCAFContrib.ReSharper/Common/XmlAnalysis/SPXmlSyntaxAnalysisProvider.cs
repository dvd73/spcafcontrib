using System.Collections.Generic;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Xml.Stages.Analysis;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.Util;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Inspection.Xml;

namespace SPCAFContrib.ReSharper.Common.XmlAnalysis
{
    [XmlAnalysisProvider]
    public class SPXmlSyntaxAnalysisProvider : IXmlAnalysisProvider
    {
        /// <summary>
        /// Check to see if it’s a file you’re interested in and add just the analyses you need
        /// </summary>
        /// <param name="file"></param>
        /// <param name="process"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public IEnumerable<JetBrains.ReSharper.Daemon.Xml.Stages.XmlAnalysis> GetAnalyses(IXmlFile file, IDaemonProcess process, IContextBoundSettingsStore settings)
        {
            if (file is IDTDFile || file.GetSourceFile().HasExcluded(settings))
                return EmptyList<JetBrains.ReSharper.Daemon.Xml.Stages.XmlAnalysis>.InstanceList;

            return new JetBrains.ReSharper.Daemon.Xml.Stages.XmlAnalysis[]
            {
                new SPElementsFileTagProblemAnalysis(new XmlTagProblemAnalyzer[]
                {
                    new DeployContentTypesCorrectly(),
                    new DeployFieldsCorrectly(),
                    new ConsiderHiddenListTemplates(),
                    new ConsiderOverwriteAttributeForContentType(),
                    new ConsiderOverwriteAttributeForField(),
                    new DeployTaxonomyFieldsCorrectly(),
                    new DoNotAllowDeletionForField(),
                    new DoNotAllowDeletionForList(),
                    new DoNotDefineMultipleContentTypeGroupInOneElementFile(),
                    new DoNotDefineMultipleFieldGroupInOneElementFile(),
                    new DoNotUseSystemListNames(),
                    new WebPartModuleDefinitionMightbeImproved(),
                    new NameWithPictureForUserField(),
                    new UniqueListInstanceUrl(),
                    new UniqueListInstanceTitle(),
                    new UniqueContentTypeID(),
                    new UniqueFieldID(),
                    new UniqueFieldName(),
                    new UniqueFieldStaticName()
                }),
                new SPListSchemaFileTagProblemAnalysis(new XmlTagProblemAnalyzer[]
                {
                    new FieldIdShouldBeUppercase(),
                    new AvoidListContentTypes(), 
                    new DeclareEmptyFieldsElement(),
                    new DoNotDeployTaxonomyFieldsInListDefinition(),
                    new EnsureFolderContentTypeInListDefinition(),
                    new NameWithPictureForUserField()
                }),
                new SPWebPartFileTagProblemAnalysis(new XmlTagProblemAnalyzer[]
                {
                    new WebPartDefinitionMightBeImproved()
                }) 
            };
        }
    }
}
