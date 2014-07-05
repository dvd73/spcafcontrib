using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.Tree;
using SPCAFContrib.ReSharper.Common.Extensions;

namespace SPCAFContrib.ReSharper.Common.XmlAnalysis
{
    public class SPXmlFileTagProblemAnalysisBase : JetBrains.ReSharper.Daemon.Xml.Stages.XmlAnalysis
    {
        private bool _isSPXmlFile = false;
        private readonly string _xmlSchemaName;
        private readonly string _xmlSchemaContainerXPath;
        private readonly IEnumerable<XmlTagProblemAnalyzer> _tagProblemAnalyzers;

        public SPXmlFileTagProblemAnalysisBase(string xmlSchemaContainerXPath, string xmlSchemaName, IEnumerable<XmlTagProblemAnalyzer> analyzers)
        {
            _xmlSchemaName = xmlSchemaName;
            _xmlSchemaContainerXPath = xmlSchemaContainerXPath;
            _tagProblemAnalyzers = analyzers;
        }

        public override bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return element is IXmlTagContainer;
        }

        public override void Init(IXmlFile file)
        {
            _isSPXmlFile = false;

            IXmlTag validatedTag = file.GetNestedTags<IXmlTag>(_xmlSchemaContainerXPath).FirstOrDefault();

            if (validatedTag != null)
            {
                // check xmlns="http://schemas.microsoft.com/sharepoint/" to be sure this is xml with sharepoint schema
                _isSPXmlFile = validatedTag.CheckAttributeValue("xmlns", _xmlSchemaName);
            }

            base.Init(file);

            if (_isSPXmlFile)
                foreach (XmlTagProblemAnalyzer xmlTagProblemAnalyzer in _tagProblemAnalyzers)
                {
                    xmlTagProblemAnalyzer.Init(file);
                }
        }

        public override void Done(IXmlFile file, IHighlightingConsumer consumer)
        {
            if (_isSPXmlFile)
                foreach (XmlTagProblemAnalyzer xmlTagProblemAnalyzer in _tagProblemAnalyzers)
                {
                    xmlTagProblemAnalyzer.Done(file, consumer);
                }
        }

        public override void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer consumer)
        {
            IXmlTag xmlTag = element as IXmlTag;
            if (xmlTag == null || !_isSPXmlFile) return;

            // run analysers for each node 
            foreach (XmlTagProblemAnalyzer xmlTagProblemAnalyzer in _tagProblemAnalyzers)
            {
                xmlTagProblemAnalyzer.Run(xmlTag, consumer);
            }
        }
    }
}
