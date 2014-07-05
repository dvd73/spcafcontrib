using System.Collections.Generic;

namespace SPCAFContrib.ReSharper.Common.XmlAnalysis
{
    public class SPWebPartFileTagProblemAnalysis : SPXmlFileTagProblemAnalysisBase
    {
        public SPWebPartFileTagProblemAnalysis(IEnumerable<XmlTagProblemAnalyzer> analyzers) :
            base("webParts/webPart", "http://schemas.microsoft.com/WebPart/", analyzers)
        {
        }
    }
}
