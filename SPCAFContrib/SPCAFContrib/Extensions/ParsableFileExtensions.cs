using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jurassic;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Extensions
{
    public static class ParsableFileExtensions
    {
        #region methods

        #region FindJScript
        public static void FindJScript(this InternalSourceFile target, bool checkScriptTag, Func<string, bool> js_validator, Func<string, bool> text_validator, Action<int, int> report)
        {
            Regex r = new Regex(@"<script(.|\n)*?script>", RegexOptions.IgnoreCase);
            if (target.FileContent.Length > 0 && (!checkScriptTag || r.IsMatch(target.FileContent)))
            {
                HtmlDocument htmlDocument = new HtmlDocument();

                bool html_validated = false;
                try
                {
                    htmlDocument.LoadHtml(target.FileContent);
                    html_validated = htmlDocument.FindJScript(js_validator, text_validator, report);
                }
                catch 
                {
                }

                if (!html_validated)
                {
                    int k = target.FileContentLines.FindJScript(text_validator);
                    if (k >= 0)
                    {
                        report(k, 0);
                    }
                }
            }
        }

        // Reserved
        //public static void FindJScript(this HTMLParsableFile target, Func<HtmlNode, bool> js_validator, Func<string, bool> text_validator, Action<int, int> report)
        //{
        //    Regex r = new Regex(@"<script(.|\n)*?script>", RegexOptions.IgnoreCase);
        //    if (target.FileContent.Length > 0 && r.IsMatch(target.FileContent))
        //    {
        //        HtmlDocument htmlDocument = target.GetAsHtmlNode();

        //        if (!htmlDocument.FindJScript(js_validator, report))
        //        {
        //            int k = target.FileContentLines.FindJScript(text_validator);
        //            if (k >= 0)
        //            {
        //                report(k, 0);
        //            }
        //        }
        //    }
        //}

        public static bool FindJScript(this HtmlDocument htmlDocument, Func<string, bool> js_validator,  Func<string, bool> text_validator, Action<int, int> report)
        {
            ScriptEngine engine = new ScriptEngine();

            bool result = false;

            if (htmlDocument != null)
            {
                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//script");
                if (htmlNodeCollection != null)
                {
                    foreach (HtmlNode scriptNode in htmlNodeCollection)
                    {
                        if (scriptNode.InnerText.Trim().Length > 0)
                        {
                            if (js_validator != null)
                                result = js_validator(scriptNode.InnerText);
                            else if (text_validator != null)
                                result = text_validator(scriptNode.InnerText);

                            if (result)
                                report(scriptNode.Line, scriptNode.LinePosition);
                        }
                    }
                }
            }

            return result;
        }

        public static int FindJScript(this string[] lines, Func<string, bool> text_validator)
        {
            List<string> list = new List<string>(lines);
            return list.FindIndex(l => text_validator(l));
        } 
        #endregion

        #region FindControl
        public static void FindControl(this InternalSourceFile target, string controlTypeName, Action<string, int, int> report)
        {
            if (target.FileContent.Length > 0 && target.FileContent.FindControl(controlTypeName))
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(target.FileContent);

                if (!htmlDocument.FindControl(controlTypeName, report))
                {
                    int k = target.FileContentLines.FindControl(controlTypeName);
                    if (k >= 0)
                    {
                        report(target.FileContentLines[k], k, 0);
                    }
                }
            }
        }

        public static bool FindControl(this HtmlDocument htmlDocument, string controlTypeName, Action<string, int, int> report)
        {
            bool result = false;

            if (htmlDocument != null)
            {
                // To match elements whose nodename ends with the controlTypeName 
                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes(String.Format("//*['{0}' = substring(name(), string-length(name()) - string-length('{0}') + 1)]", controlTypeName.ToLower()));
                if (htmlNodeCollection != null)
                {
                    foreach (HtmlNode scriptNode in htmlNodeCollection)
                    {
                        if (scriptNode.OuterHtml.Trim().Length > 0)
                        {
                            result = true;
                            report(scriptNode.OuterHtml, scriptNode.Line, scriptNode.LinePosition);
                        }
                    }
                }
            }

            return result;
        }

        public static int FindControl(this string[] lines, string controlTypeName)
        {
            List<string> list = new List<string>(lines);
            return list.FindIndex(l => l.FindControl(controlTypeName));
        }

        public static bool FindControl(this string line, string controlTypeName) 
        {
            Regex r = new Regex(String.Format(@"<\w+:{0}(.|\n)*?:{0}>", controlTypeName), RegexOptions.IgnoreCase);
            return r.IsMatch(line);
        }
        #endregion

        #endregion
        
        #region validators
        public static bool FindJQueryVariableByEngine(this string line)
        {
            ScriptEngine engine = new ScriptEngine();
            bool result = false;

            try
            {
                bool evaluated = engine.Evaluate<bool>(line);
                if (evaluated && engine.HasGlobalValue("$"))
                {
                    result = true;
                }
            }
            catch (JavaScriptException ex)
            {
                if (ex.Message.Equals("ReferenceError: $ is not defined",
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool FindJQueryVariableByIndexOf(this string line)
        {
            return line.IndexOf("$.", StringComparison.OrdinalIgnoreCase) >= 0 ||
                line.IndexOf("$(", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool FindJQueryDocumentReadyByIndexOf(this string line)
        {
            Regex r1 = new Regex(@"\([\s]*?document[\s]*?\)\.ready", RegexOptions.IgnoreCase);
            Regex r2 = new Regex(@"\w*uery\b[\s]*?\([\s]*?function[\s]*?\([\s]*?\)", RegexOptions.IgnoreCase);
            Regex r3 = new Regex(@"\$[\s]*?\([\s]*?function[\s]*?\([\s]*?\)", RegexOptions.IgnoreCase);

            return r1.IsMatch(line) || r2.IsMatch(line) || r3.IsMatch(line);
        } 
        #endregion
    }
}
