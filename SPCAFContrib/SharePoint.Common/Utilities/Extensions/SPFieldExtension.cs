using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml.Linq;
using Microsoft.SharePoint.Utilities;
using System.Threading;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPFieldExtension
    {
        /// <summary>
        /// Usage: 
        /// Guid fieldGuid = new Guid("put-your-field-guid-here");
        /// SPListItem listItem = // get the list item from somewhere
        /// var mappedValue = listItem.Fields[fieldGuid].GetMappedValue(listItem[fieldGuid]);
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetMappedValue(this SPFieldMultiChoice field, object value)
        {
            return GetValueFromMapping(field.Mappings, Convert.ToString(value));
        }

        internal static string GetValueFromMapping(string mappingsXml, string fieldMultiValue)
        {
            var result = String.Empty;
            foreach (var value in fieldMultiValue.Split(new string[] { ";#" }, StringSplitOptions.RemoveEmptyEntries))
            {
                XDocument document = XDocument.Parse(mappingsXml);
                var mapping = document.Element("MAPPINGS").Elements("MAPPING").FirstOrDefault(m => LocalizedEqual(m.Value, value));
                if (mapping != null)
                    result += ";#" + mapping.Attribute("Value").Value;
            }

            return result.TrimStart(';', '#');
        }

        private static bool LocalizedEqual(string mappingValue, string value)
        {
            if (mappingValue.TrimStart().StartsWith("$"))
                mappingValue = SPUtility.GetLocalizedString(mappingValue, "core", (uint)Thread.CurrentThread.CurrentUICulture.LCID);

            return mappingValue.Equals(value);
        }
    }
}
