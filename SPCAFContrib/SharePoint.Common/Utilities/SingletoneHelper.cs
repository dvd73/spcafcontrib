using System;
using System.Diagnostics.CodeAnalysis;

namespace SharePoint.Common.Utilities
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
                        // Office 365 has a restriction of System.Threading usage, but regular sandbox - no.
                        //System.Threading.Thread.MemoryBarrier();
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
