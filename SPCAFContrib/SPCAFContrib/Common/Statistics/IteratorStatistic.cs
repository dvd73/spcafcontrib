using System.Collections.Generic;
using Mono.Cecil;
using SPCAFContrib.Common.Statistics.Base;

namespace SPCAFContrib.Common.Statistics
{
    public class IteratorStatistic : StatisticBase
    {
        #region Singeton interface

        public new static IteratorStatistic Instance
        {
            get { return GetInstance<IteratorStatistic>(); }
        }

        #endregion 

        public List<string> CustomTypes { get; set; }

        private bool _collected = false;
        public override bool Collected {
            get { return _collected; }
            set {  } 
        }

        public void SetCollected(bool value)
        {
            _collected = value;
        }
    }
}
