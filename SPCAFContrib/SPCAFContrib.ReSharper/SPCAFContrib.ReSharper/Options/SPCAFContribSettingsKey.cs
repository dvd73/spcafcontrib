using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store;
using JetBrains.ReSharper.Settings;

namespace SPCAFContrib.ReSharper.Options
{
    [SettingsKey(typeof(CodeInspectionSettings), "SPCAF Contrib settings")]
    public class SPCAFContribSettingsKey
    {
        [SettingsIndexedEntry("List of ignored JavasSript files")]
        public IIndexedEntry<string, string> IgnoredJsFiles { get; set; }

        [SettingsIndexedEntry("List of ignored Xml files")]
        public IIndexedEntry<string, string> IgnoredXmlFiles { get; set; }

        [SettingsIndexedEntry("List of other ignored files")]
        public IIndexedEntry<string, string> IgnoredOtherFiles { get; set; }
    }
}