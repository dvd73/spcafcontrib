using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.JavaScript.LanguageImpl;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.Util;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Options;
using JetBrains.ReSharper.Psi.Html;

namespace SPCAFContrib.ReSharper.Common.Extensions
{
    public static class IPsiSourceFileExtension
    {
        public static bool HasExcluded(this IPsiSourceFile file, IContextBoundSettingsStore settings)
        {
            if (settings == null)
            {
                IEnumerable<string> a = new string[0];

                if (file.PrimaryPsiLanguage.Is<JavaScriptLanguage>() || file.PrimaryPsiLanguage.Is<HtmlLanguage>())
                {
                    a = a.Union(ExcludedFiles.JavaScript.SplitByNewLine());
                }
                else if (file.PrimaryPsiLanguage.Is<XmlLanguage>())
                {
                    a = a.Union(ExcludedFiles.Xml.SplitByNewLine());
                }

                return
                    a.Union(ExcludedFiles.Other.SplitByNewLine())
                        .Any(
                            f =>
                            {
                                string trimmed = f.Trim();
                                return !String.IsNullOrEmpty(trimmed) &&
                                       (new Regex(trimmed, RegexOptions.IgnoreCase).IsMatch(file.Name) ||
                                        new Wildcard(trimmed, RegexOptions.IgnoreCase).IsMatch(file.Name));
                            });
            }
            else
            {
                bool result = false;

                if (file.PrimaryPsiLanguage.Is<JavaScriptLanguage>() || file.PrimaryPsiLanguage.Is<HtmlLanguage>())
                {
                    result |= FileShouldBeIgnored(settings, file.Name, SPCAFContribSettingsAccessor.IgnoredJsFileMasks);
                }
                else if (file.PrimaryPsiLanguage.Is<XmlLanguage>())
                {
                    result |= FileShouldBeIgnored(settings, file.Name, SPCAFContribSettingsAccessor.IgnoredXmlFileMasks);
                }
                
                result |= FileShouldBeIgnored(settings, file.Name, SPCAFContribSettingsAccessor.IgnoredOtherFileMasks);

                return result;
            }
        }

        private static bool FileShouldBeIgnored(
            IContextBoundSettingsStore settings, 
            string fileName, 
            Expression<Func<SPCAFContribSettingsKey, IIndexedEntry<string, string>>> expression)
        {
            bool result = false;
            
            foreach (string indexedValue in settings.EnumIndexedValues(expression))
            {
                string trimmed = indexedValue.Trim();
                if (!String.IsNullOrEmpty(trimmed) &&
                    (new Regex(trimmed, RegexOptions.IgnoreCase).IsMatch(fileName) ||
                     new Wildcard(trimmed, RegexOptions.IgnoreCase).IsMatch(fileName)))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
