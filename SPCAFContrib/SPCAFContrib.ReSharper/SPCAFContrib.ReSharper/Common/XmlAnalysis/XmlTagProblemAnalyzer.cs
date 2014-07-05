using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace SPCAFContrib.ReSharper.Common.XmlAnalysis
{
    public abstract class XmlTagProblemAnalyzer
    {
        public abstract void Run(IXmlTag element, IHighlightingConsumer consumer);

        public virtual void Init(IXmlFile file)
        {
            
        }

        public virtual void Done(IXmlFile file, IHighlightingConsumer consumer)
        {

        }
    }
}
