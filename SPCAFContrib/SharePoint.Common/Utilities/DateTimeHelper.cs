using System;

namespace SharePoint.Common.Utilities
{
    public static class DateTimeHelper
    {
        public static DateTime CheckForNull(object o, DateTime defaultValue)
        {
            if (o == null || Convert.IsDBNull(o) || o.ToString().Length == 0) return defaultValue;
            else return Convert.ToDateTime(o);
        }
    }
}
