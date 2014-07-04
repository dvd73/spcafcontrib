using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPFeatureExtension
    {
        public static string GetPropertyValue(this SPFeature feature, string name)
        {
            if (feature.Properties[name] != null)
                return feature.Properties[name].Value;
            else return null;
        }

        public static List<string> GetPropertyAsList(this SPFeature feature, string name)
        {
            if (feature.Properties[name] != null)
                return feature.Properties[name].Value.Split().ToList();
            else return new List<string>();
        }

        public static string PropertyAsString(this SPFeature feature, string name, string defaultVal)
        {
            string value = feature.GetPropertyValue(name);
            return value != null ? value : defaultVal;
        }

        public static string PropertyAsString(this SPFeature feature, string name)
        {
            string value = feature.GetPropertyValue(name);
            return value;
        }

        public static bool? PropertyAsBool(this SPFeature feature, string name)
        {
            string value = feature.GetPropertyValue(name);
            if (value != null)
                return Convert.ToBoolean(value);
            return null;
        }

        public static bool? PropertyAsBool(this SPFeature feature, string name, bool defaultValue)
        {
            bool? value = feature.PropertyAsBool(name);
            return value.HasValue ? value : defaultValue;
        }

        public static int? PropertyAsInt(this SPFeature feature, string name, int defaultVal)
        {
            return feature.PropertyAsInt(name).HasValue ?
                   feature.PropertyAsInt(name).Value :
                    defaultVal;
        }

        public static int? PropertyAsInt(this SPFeature feature, string name)
        {
            string value = feature.GetPropertyValue(name);
            if (value != null)
                return Convert.ToInt32(value);
            return null;
        }

        public static Guid? PropertyAsGuid(this SPFeature feature, string name)
        {
            string value = feature.GetPropertyValue(name);
            if (value != null)
                try
                {
                    return new Guid(value);
                }
                catch { return null; };
            return null;
        }

        public static Guid? PropertyAsGuid(this SPFeature feature, string name, Guid defaultValue)
        {
            Guid? value = feature.PropertyAsGuid(name);
            return value.HasValue ? value : defaultValue;
        }

        public static string PropertyAsSPUrl(this SPFeature feature, SPWeb web, string name, string defaultValue)
        {
            string url = feature.GetPropertyValue(name);
            if (url != null)
            {
                url = web.SharePointUrlToAbsoluteUrl(url);
                if (url.ToLower().Contains("://"))
                {
                    url = url.Replace("//", "/");
                    url = url.Replace(":/", "://");
                }
                else
                {
                    url = url.Replace("//", "/");
                }
                return url;
            }
            else return defaultValue;
        }

        /// <summary>
        /// Returns 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static List<string> PropertyAsList(this SPFeature feature, string name, List<string> defaultVal)
        {
            List<string> list = feature.PropertyAsList(name);
            return list.Count == 0 ? defaultVal : list;
        }

        public static List<string> PropertyAsList(this SPFeature feature, string name)
        {
            string slist = feature.GetPropertyValue(name);
            return slist.StringToList();
        }

        public static List<string> PropertyAsList(this SPFeature feature, string name, string separator)
        {
            string slist = feature.GetPropertyValue(name);
            return slist.StringToList(separator);
        }

        /// <summary>
        /// Returns 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static Dictionary<string, string> PropertyAsDictionary(this SPFeature feature, string name, Dictionary<string, string> defaultVal)
        {
            Dictionary<string, string> dictionary = feature.PropertyAsDictionary(name);
            return dictionary.Count == 0 ? defaultVal : dictionary;
        }

        public static Dictionary<string, string> PropertyAsDictionary(this SPFeature feature, string name)
        {
            string stringDictionary = feature.GetPropertyValue(name);
            return stringDictionary.StringToDictionary();
        }
    }
}
