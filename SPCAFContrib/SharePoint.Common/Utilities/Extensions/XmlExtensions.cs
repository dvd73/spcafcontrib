using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml.Linq;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class XmlExtensions
    {
        public static string InnerText(this XElement el, SaveOptions saveOptions)
        {
            StringBuilder str = new StringBuilder();
            foreach (XNode element in el.Elements())
            {
                str.Append(element.ToString(saveOptions));
            }
            return str.ToString();
        }
    }
}
