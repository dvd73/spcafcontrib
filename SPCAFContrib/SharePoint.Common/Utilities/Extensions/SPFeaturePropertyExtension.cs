using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPFeaturePropertyExtension
    {
        public static int GetInt32(this SPFeatureProperty property)
        {
            if (property == null)
                return 0;
            int i = 0;
            int.TryParse(property.Value, out i);
            return i;
        }

        public static List<string> GetListOfString(this SPFeatureProperty property)
        {
            if (property == null)
                return new List<string>();
            return property.Value.Split(',').ToList();
        }

        public static List<T> AsList<T>(this SPFeatureProperty property, Func<string, T> selector)
        {
            if (property == null)
                return new List<T>();
            return property.Value.Split().Select(selector).ToList();
        }

        public static List<T> AsList<T>(this SPFeatureProperty property, char separator, Func<string, T> selector)
        {
            if (property == null)
                return new List<T>();
            return property.Value.Split(separator).Select(selector).ToList();
        }

        public static List<T> AsList<T>(this SPFeatureProperty property, string separator, Func<string, T> selector)
        {
            if (property == null)
                return new List<T>();
            return property.Value.Split(separator).Select(selector).ToList();
        }

        public static double GetDouble(this SPFeatureProperty property)
        {
            try
            {
                if (property == null)
                    return 0.0;
                return Convert.ToDouble(property.Value);
            }
            catch
            {
                return 0.0;
            }
        }
    }
}
