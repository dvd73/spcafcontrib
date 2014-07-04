using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Extensions
{
    public static class WebPartFileExtensions
    {
        #region methods

        public static bool HasProperty(this WebPartFile webPartFile, string propName)
        {
            if (webPartFile.Properties == null)
                return false;

            return webPartFile.Properties.ContainsKey(propName);
        }


        public static string GetPropertyValue(this WebPartFile file, string propName)
        {
            if (!file.HasProperty(propName))
                return string.Empty;

            return file.Properties[propName];
        }


        #endregion
    }
}
