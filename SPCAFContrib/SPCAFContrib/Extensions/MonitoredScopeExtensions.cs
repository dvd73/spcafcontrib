using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAFContrib.Utils;

namespace SPCAFContrib.Extensions
{
    public static class MonitoredScopeExtensions
    {
        #region methods

        public static void WithMonitoredScope(this object sender, string scopeName, Action<MonitoredScope> action)
        {
            using (MonitoredScope scope = new MonitoredScope(scopeName))
            {
                action(scope);
            }
        }

        #endregion
    }
}
