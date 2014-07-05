using System.Collections.Generic;

namespace SPCAFContrib.ReSharper.Common.CodeAnalysis
{
    public class MethodCriteria
    {
        public string ShortName { get; set; }
        public IEnumerable<ParameterCriteria> Parameters { get; set; }
    }
}
