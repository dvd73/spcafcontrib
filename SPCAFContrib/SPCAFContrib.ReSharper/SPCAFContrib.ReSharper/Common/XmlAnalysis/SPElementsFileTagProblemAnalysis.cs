using System.Collections.Generic;

namespace SPCAFContrib.ReSharper.Common.XmlAnalysis
{
    public class SPElementsFileTagProblemAnalysis : SPXmlFileTagProblemAnalysisBase
    {
        public SPElementsFileTagProblemAnalysis(IEnumerable<XmlTagProblemAnalyzer> analyzers) :
            base("Elements", "http://schemas.microsoft.com/sharepoint", analyzers)
        {
        }
    }
}
