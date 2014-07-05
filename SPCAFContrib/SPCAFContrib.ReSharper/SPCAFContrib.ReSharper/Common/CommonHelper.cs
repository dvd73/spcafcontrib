using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SPCAFContrib.ReSharper.Common
{
    internal class CommonHelper
    {
        public static string NormalizeQFMessage(StringBuilder sb)
        {
            string s = sb.ToString();
            if (Regex.Matches(s, Environment.NewLine).Count == 1)
                return s.Replace(".", "").Trim();
            else
                return s.Trim();
        }
    }
}
