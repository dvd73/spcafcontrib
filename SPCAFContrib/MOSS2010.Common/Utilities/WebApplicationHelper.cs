using System.Web.Configuration;
using Microsoft.SharePoint.Administration;

namespace MOSS.Common.Utilities
{
    public static class WebApplicationHelper
    {
        const string webConfigFileName = "web.config";
        const string rootPath = "/";

        public static WebConfigurationFileMap CreateConfigurationFileMap(SPWebApplication webApplication)
        {
            var fileMap = new WebConfigurationFileMap();

            var webApplicationPath = webApplication.GetIisSettingsWithFallback(SPUrlZone.Default).Path.FullName;

            // Create a VirtualDirectoryMapping object to use
            // as the default directory for all the virtual 
            // directories.
            var vDirMapBase = new VirtualDirectoryMapping(webApplicationPath, true, webConfigFileName);

            // Add it to the virtual directory mapping collection.
            fileMap.VirtualDirectories.Add(rootPath, vDirMapBase);

            return fileMap;
        }
        
    }
}
