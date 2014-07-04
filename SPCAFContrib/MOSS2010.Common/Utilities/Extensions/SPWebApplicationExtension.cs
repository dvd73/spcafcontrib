using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace MOSS.Common.Utilities.Extensions
{
    public static class SPWebApplicationExtension
    {
        const string rootPath = "/";

        /// <summary>
        /// Gets the configuration (from web.config) of the Default Zone of the web application
        /// </summary>
        /// <param name="webApplication"></param>
        /// <returns></returns>
        public static System.Configuration.Configuration GetConfiguration(this SPWebApplication webApplication)
        {
            return WebConfigurationManager.OpenMappedWebConfiguration(WebApplicationHelper.CreateConfigurationFileMap(webApplication), rootPath);
        }

        /// <summary>
        /// Gets a connection string from the configuration (web.config) of the Default Zone of the web application
        /// </summary>
        /// <param name="webApplication"></param>
        /// <param name="name">Connection string name</param>
        /// <returns></returns>
        public static string GetConnectionString(this SPWebApplication webApplication, string name)
        {
            var config = webApplication.GetConfiguration();

            var connectionStringSettings = config.ConnectionStrings.ConnectionStrings[name];
            if (connectionStringSettings != null)
                return connectionStringSettings.ConnectionString;
            else
                return null;
        }

        /// <summary>
        /// Gets an AppSettings value from the configuration (web.config) of the Default Zone of the web application
        /// </summary>
        /// <param name="webApplication"></param>
        /// <param name="key">AppSettings key</param>
        /// <returns></returns>
        public static string GetAppSettingsValue(this SPWebApplication webApplication, string key)
        {
            var config = webApplication.GetConfiguration();

            var appSetting = config.AppSettings.Settings[key];

            if (appSetting != null)
                return appSetting.Value;
            else
                return null;
        }

        /// <summary>
        /// Activate a list of features
        /// </summary>
        /// <param name="app">Web to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void ActivateFeatures(this SPWebApplication app, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = app.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid featureId in featureIds.Except(existingFeatures))
            {
                app.Features.Add(featureId);
            }
        }

        /// <summary>
        /// Deactivate a list of features
        /// </summary>
        /// <param name="app">Web object to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void DeactivateFeatures(this SPWebApplication app, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = app.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid existingFeature in existingFeatures.Intersect(featureIds))
            {
                app.Features.Remove(existingFeature);
            }
        }
    }
}
