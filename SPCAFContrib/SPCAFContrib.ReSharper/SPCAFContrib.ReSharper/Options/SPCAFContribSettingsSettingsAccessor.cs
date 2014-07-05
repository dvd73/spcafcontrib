using System;
using System.Linq.Expressions;
using JetBrains.Application.Settings.Store;

namespace SPCAFContrib.ReSharper.Options
{
    public static class SPCAFContribSettingsAccessor
    {
        public static readonly Expression<Func<SPCAFContribSettingsKey, IIndexedEntry<string, string>>> 
            IgnoredJsFileMasks = key => key.IgnoredJsFiles;

        public static readonly Expression<Func<SPCAFContribSettingsKey, IIndexedEntry<string, string>>>
            IgnoredXmlFileMasks = key => key.IgnoredXmlFiles;

        public static readonly Expression<Func<SPCAFContribSettingsKey, IIndexedEntry<string, string>>>
            IgnoredOtherFileMasks = key => key.IgnoredOtherFiles;

        static SPCAFContribSettingsAccessor()
        {
            
        }
    }
}
