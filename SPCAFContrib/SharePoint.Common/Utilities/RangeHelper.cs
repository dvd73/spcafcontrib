using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Common.Utilities
{
    public static class RangeHelper
    {
        public static IEnumerable<int> To(this int from, int to)
        {
            for (int i = from; i <= to; i++)
                yield return i;
        }
        public static IEnumerable<int> To(this int from, int to, int step)
        {
            for (int i = from; i <= to; i = i + step)
                yield return i;
        }
        public static void Do(this IEnumerable<int> integers, Action<int> action)
        {
            foreach (int i in integers)
                action(i);
        }
    }
}
