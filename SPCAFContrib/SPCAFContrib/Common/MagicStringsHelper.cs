using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SPCAFContrib.Common
{
    public static class MagicStringsHelper
    {
        private static Dictionary<string, Regex> magicStringTypes= new Dictionary<string, Regex>
        {
            {"Uri", new Regex(@"https?:\/(\/([\da-z\.-]+))+\/?",RegexOptions.Compiled)},
            {"Email", new Regex(@"([a-z0-9_\.-]+)@([a-z0-9_\.-]+)\.([a-z\.]{2,6})",RegexOptions.Compiled)},
            {"Path", new Regex(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$",RegexOptions.Compiled)},
            {"AccountName", new Regex(@"^([a-z][a-z0-9.-]+)\\(?![\x20.]+$)([^\\/""[\]:|<>+=;,?*@]+)$",RegexOptions.Compiled)}
        };

        public static string Match(string value)
        {
            if (!string.IsNullOrEmpty(value) && !IsExcluded(value))
            {
                return (from pair in magicStringTypes 
                        where pair.Value.IsMatch(value) 
                        select pair.Key).FirstOrDefault();
            }
            return null;
        }

        private static bool IsExcluded(string value)
        {
            //Exclude all schemas
            if (value.StartsWith("http://schemas.microsoft.com"))
            {
                return true;
            }
            return false;
        }

        
    }
}
