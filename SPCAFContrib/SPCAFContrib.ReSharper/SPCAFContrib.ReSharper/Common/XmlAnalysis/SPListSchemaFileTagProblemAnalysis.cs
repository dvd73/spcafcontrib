using System.Collections.Generic;

namespace SPCAFContrib.ReSharper.Common.XmlAnalysis
{
    public class SPListSchemaFileTagProblemAnalysis : SPXmlFileTagProblemAnalysisBase
    {
        public SPListSchemaFileTagProblemAnalysis(IEnumerable<XmlTagProblemAnalyzer> analyzers) : 
            base("List", "http://schemas.microsoft.com/sharepoint", analyzers)
        {
        }
    }
}
