using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPFeaturePropertyCollectionExtension
    {
        public static IEnumerable<SPFeatureProperty> AsEnumerable(this SPFeaturePropertyCollection properties)
        {
            return properties.OfType<SPFeatureProperty>();
        }

        public static SPFeatureProperty GetProperty(this SPFeaturePropertyCollection properties, string name)
        {
            return properties.AsEnumerable().FirstOrDefault(p => p.Name == name);
        }

        public static void UseValue(this SPFeaturePropertyCollection properties, string name, Action<string> action)
        {
            var property = properties.GetProperty(name);
            if (property != null)   
                action(property.Value);
        }

        public static void UseProperty(this SPFeaturePropertyCollection properties, string name, Action<SPFeatureProperty> action)
        {
            var property = properties.GetProperty(name);
            if (property != null)
                action(property);
        }

        public static T GetValue<T>(this SPFeaturePropertyCollection properties, string name, Func<string, T> getter)
        {
            var property = properties.GetProperty(name);
            if (property != null)
                return getter(property.Value);
            else
                return default(T);
        }
    }
}
