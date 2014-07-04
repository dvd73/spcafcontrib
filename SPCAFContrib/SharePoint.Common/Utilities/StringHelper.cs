using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SharePoint.Common.Utilities
{
    public static class TextModificationHelpers
    {
        /// <summary>
        /// Cuts the string to the specified length and appends a space and ellipsis ("...")
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Ellipsize(this string text, int maxLength)
        {
            string result;
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
                int lastSpaceCharIndex = text.LastIndexOf(' ');
                if (lastSpaceCharIndex > 0)
                {
                    result = text.Substring(0, lastSpaceCharIndex);
                }
                else
                {
                    result = text;
                }
                result = result + " ...";
            }
            else
            {
                result = text;
            }
            return result;
        }
    }

    public static class StringHelper
    {   
        private static Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

        public static bool ParseGuid(string candidate, out Guid output)
        {
            bool isValid = false;

            output = Guid.Empty;

            if (candidate != null)
            {
                if (isGuid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }

            return isValid;
        }

        public static string StripHtmlTags(string input)
        {
            return Regex.Replace(input, @"(<|(<\s+))+[^>]+((\s+>)|>)+", string.Empty);
        }

        public static string StripBBCode(string input)
        {
            return Regex.Replace(input, @"(\[|(\[\s+))+[^\]]+((\s+\])|\])+", string.Empty);
        }

        public static string CheckForNull(object o)
        {
            if (o == null || Convert.IsDBNull(o)) return String.Empty;
            else return Convert.ToString(o);
        }
        public static bool IsEmail(string stringToCheck)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9_%+-]+\.?[a-zA-Z0-9_%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", RegexOptions.Compiled);
            return (!string.IsNullOrEmpty(stringToCheck) && regex.IsMatch(stringToCheck));
        }
    }
}
