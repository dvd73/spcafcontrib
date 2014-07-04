using System.Collections.Generic;
using Mono.Cecil;

namespace SPCAFContrib.Common.Statistics.Base
{
    public class StatisticBase : SingletoneHelper
    {
        #region Singeton interface

        public static StatisticBase Instance
        {
            get { return GetInstance<StatisticBase>(); }
        }

        #endregion 

        public virtual bool Collected { get; set; }
        public List<AssemblyDefinition> SolutionLibs { get; set; }
        public List<AssemblyDefinition> ResolvedLibs { get; set; }
        public List<string> UnresolvedLibs { get; set; }
    }
}
