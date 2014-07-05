using System.IO;
using System.Reflection;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.Util;

namespace SPCAFContrib.ReSharper.Common.Shell
{
    [ShellComponent]
    public class PredefinedSPCAFContribSettings : IHaveDefaultSettingsStream
    {
        public Stream GetDefaultSettingsStream(Lifetime lifetime)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SPCAFContrib.ReSharper.Options.PredefinedSPCAFContribSettings.xml");
            Assertion.AssertNotNull(stream, "stream == null");
            lifetime.AddDispose(stream);

            return stream;
        }


        string IHaveDefaultSettingsStream.Name
        {
            get
            {
                return "Predefined SPCAF Contrib Settings";
            }
        }
    }
}
