using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SharePoint.Common.Utilities.Extensions;

namespace MOSS.Common.Utilities.Extensions
{
    public static class SPFeatureReceiverPropertiesExtension
    {
        public static List<string> GetElementFileNames(this SPFeatureReceiverProperties properties)
        {
            var featureDefinitionXml = properties.Definition.GetXmlDefinition(System.Threading.Thread.CurrentThread.CurrentCulture);
            XDocument doc = XDocument.Parse(featureDefinitionXml.OuterXml);
            var fullNames = GetElementFileNames(doc).Select(name => Path.Combine(properties.Feature.Definition.RootDirectory, name));
            return fullNames.ToList();
        }

        private static List<string> GetElementFileNames(XDocument featureXml)
        {
            List<string> names = new List<string>();

            var manifests = featureXml.Descendants("ElementManifests").FirstOrDefault();

            if (manifests != null)
            {
                var elementFile = manifests.Descendants("ElementFile").FirstOrDefault();

                if (elementFile != null)
                    names.Add(elementFile.Attribute("Location").Value);
            }
            return names;
        }

        public static void DeleteFeatureFiles(this SPFeatureReceiverProperties properties, SPWeb web, string elementsPath)
        {
            XDocument elementsXml = XDocument.Load(elementsPath);
            XNamespace sharePointNamespace = "http://schemas.microsoft.com/sharepoint/";

            // get each module name and the files in it
            var moduleList = from module in elementsXml.Root.Elements(sharePointNamespace + "Module")
                             select new
                             {
                                 ModuleUrl = (module.Attributes("Url").Any()) ? module.Attribute("Url").Value : null,
                                 Files = module.Elements(sharePointNamespace + "File")
                             };

            // iterate through each module and delete the child files
            foreach (var module in moduleList)
            {
                DeleteModuleFiles(module.ModuleUrl, module.Files, web);
            }
        }

        private static void DeleteModuleFiles(string moduleUrl, IEnumerable<XElement> fileList, SPWeb web)
        {
            // delete each file in the module
            foreach (var fileElement in fileList)
            {
                // use the name attribute if specified otherwise use Url attribute (since it is required)
                string filename = (fileElement.Attributes("Name").Any()) ? fileElement.Attribute("Name").Value
                    : fileElement.Attribute("Url").Value;

                // pass the moduleUrl if it has a value
                if (moduleUrl.Contains("List_Pages_UrlName"))
                    moduleUrl = SPUtility.GetLocalizedString("$Resources:List_Pages_UrlName", "cmscore", SPContext.Current.RegionalSettings.LocaleId);
                string fileUrl = !string.IsNullOrEmpty(moduleUrl) ? SPUrlUtility.CombineUrl(moduleUrl, filename) : filename;
                try
                {
                    if (!string.IsNullOrEmpty(moduleUrl))
                        web.GetFile(fileUrl).Delete();
                    else
                        web.Files.Delete(fileUrl);
                }
                catch (Exception ex)
                {
                    ex.LogError(String.Format("Невозможно удалить файл {0} по причине", fileUrl));
                }
            }
        }
    }
}
