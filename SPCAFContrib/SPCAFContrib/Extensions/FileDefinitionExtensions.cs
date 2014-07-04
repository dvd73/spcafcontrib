using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Extensions
{
    public static class FileDefinitionExtensions
    {
        #region methods

        public static bool HasProperty(this FileDefinition file, string propName)
        {
            if (file.Items == null)
                return false;

            IEnumerable<PropertyValueAttributeDefinition> properties = file.Items.OfType<PropertyValueAttributeDefinition>();

            if (properties.Count() == 0)
                return false;

            return properties.Any(p => p.Name == propName);
        }

        public static string GetPropertyValue(this FileDefinition file, string propName)
        {
            if (!file.HasProperty(propName))
                return string.Empty;

            IEnumerable<PropertyValueAttributeDefinition> properties = file.Items.OfType<PropertyValueAttributeDefinition>();
            PropertyValueAttributeDefinition prop = properties.FirstOrDefault(p => p.Name == propName);

            if (prop == null)
                return string.Empty;

            return prop.Value;
        }

        #endregion
    }
}
