using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// string.Format extension
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, object arg0)
        {
            return string.Format(format, arg0);
        }
        /// <summary>
        /// string.Format extension
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">e.g. avain1=arvo1;avain2=arvo2;avain3=arvo3;avain4=arvo4</param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(this string str)
        {
            return StringToDictionary(str, ';', '=');
        }


        /// <summary>
        /// Transforms String -> Dictionary<string,string>
        /// </summary>
        /// <param name="str">e.g. avain1=arvo1;avain2=arvo2;avain3=arvo3;avain4=arvo4</param>
        /// <param name="separator">e.g. ";" --> KeyValuPair1;KeyValuPair2;KeyValuPair3;</param>
        /// <param name="valueDelim">e.g. "=" --> key=value</param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(this string str, char separator, char valueDelim)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string[] tokens = str.Split(new char[] { separator });
            foreach (string item in tokens)
            {
                string token = item.Trim();
                // expected format [key]=[value], where key and value contains or more chars.
                if (token.Length < 3) continue;

                string[] pair = token.Split(new char[] { valueDelim });
                if (pair.Length == 2)
                {
                    string value = pair[1].Trim();
                    // trim quotes
                    value = ((value.EndsWith("'") && value.StartsWith("'"))
                            || (value.EndsWith("\"") && value.StartsWith("\""))) ?
                            value.Substring(1, value.Length - 2) : value;
                    ret.Add(pair[0], value);
                }
                else
                {
                    throw new Exception(string.Format("Error in token \"{0}\".", token));
                }
            }
            return ret;
        }

        /// <summary>
        /// Transforms String -> List<string>
        /// </summary>
        /// <param name="str">e.g. Arvo1;Arvo2;Arvo3</param>
        /// <returns></returns>
        public static List<string> StringToList(this string str)
        {
            string slist = "" + str;
            string delim = ";"; // default delimiter

            if (slist.Length > 4 &&
                // first char before ### sets the delimiter
                // for example:
                // $###item1$item2 --> list [item1, item2]
                slist.Substring(0, 4).EndsWith("###"))
            {
                delim = "" + slist[0];
                slist = slist.Substring(4);
            }
            return StringToList(slist, delim);
        }

        /// <summary>
        /// Transforms String -> List<string>
        /// </summary>
        /// <param name="str">e.g. Arvo1;Arvo2;Arvo3</param>
        /// <param name="token">e.g. ";" --> Arvo1;Arvo2;Arvo3</param>
        /// <returns></returns>
        public static List<string> StringToList(this string str, string separator)
        {
            List<string> glist = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] list = str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in list) glist.Add(s.Trim());
            }
            return glist;
        }

        /// An extension to string.split methods. Separator = ";"
        /// </summary>
        /// <param name="str">e.g. Arvo1;Arvo2;Arvo3</param>
        /// <returns></returns>
        public static string[] Split(this string str)
        {
            return str.Split(";", StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// An extension to string.split methods. 
        /// </summary>
        /// <param name="str">e.g. Arvo1;Arvo2;Arvo3</param>
        /// <param name="token">e.g. ";" --> Arvo1;Arvo2;Arvo3</param>
        /// <returns></returns>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// An extension to string.split methods. 
        /// </summary>
        /// <param name="str">e.g. Arvo1;Arvo2;Arvo3</param>
        /// <param name="token">e.g. ";" --> Arvo1;Arvo2;Arvo3</param>
        /// <returns></returns>
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            string[] separatorArr = new string[] { separator };
            return str.Split(separatorArr, options);
        }

        /// <summary>
        /// Returns true if the string is null, empty or consists entirely of whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyAfterTrim(this string value)
        {
            if (!string.IsNullOrEmpty(value))
                return string.IsNullOrEmpty(value.Trim());
            else
                return true;
        }

        /// <summary>
        /// Extension method that allows for case-insensitive string.Contains()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            return str.IndexOf(value, comparisonType) >= 0;
        }

        /// <summary>
        /// Gets an array of bytes representing the string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">The encoding used in the string</param>
        /// <returns></returns>
        public static byte[] GetBytes(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Gets an array of bytes representing the UTF8-encoded string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytesUTF8(this string str)
        {
            return str.GetBytes(new UTF8Encoding(true));
        }
    }
}
