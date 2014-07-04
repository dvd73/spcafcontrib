using System.Collections.Generic;
using Mono.Cecil;
using SPCAFContrib.Common.Statistics.Base;

namespace SPCAFContrib.Common.Statistics
{
    public class SPPersistedObjectStatistic : StatisticBase
    {
        #region Singeton interface

        public new static SPPersistedObjectStatistic Instance
        {
            get { return GetInstance<SPPersistedObjectStatistic>(); }
        }

        #endregion 

        public List<string> CustomTypes { get; set; }
        public List<string> AllowedRecivers { get; set; }
        public MultiValueDictionary<string, string> AllowedReciverCalls { get; set; }

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
