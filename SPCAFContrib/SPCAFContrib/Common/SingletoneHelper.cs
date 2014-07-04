using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCAFContrib.Common
{
    public class SingletoneHelper
    {
        private static class Storage<T>
        {
            internal static T s_instance;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2002")]
        private static T getInstance<T>(Func<T> op)
        {
            if (Storage<T>.s_instance == null)
            {
                lock (typeof(Storage<T>))
                {
                    if (Storage<T>.s_instance == null)
                    {
                        T temp = op();
                        System.Threading.Thread.MemoryBarrier();
                        Storage<T>.s_instance = temp;
                    }
                }
            }
            return Storage<T>.s_instance;
        }

        public static T GetInstance<T>()
          where T : new()
        {
            return getInstance(() => new T());
        }
    }
}
