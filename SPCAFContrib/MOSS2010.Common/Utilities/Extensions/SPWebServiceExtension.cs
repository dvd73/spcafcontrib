using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;

namespace MOSS.Common.Utilities.Extensions
{
    public static class SPWebServiceExtension
    {
        /// <summary>
        /// Activate a list of features
        /// </summary>
        /// <param name="service">Service object to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void ActivateFeatures(this SPWebService service, IEnumerable<Guid> featureIds)
        {
            var existingFeatures = service.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (var featureId in featureIds.Except(existingFeatures))
            {
                service.Features.Add(featureId);
            }
        }

        /// <summary>
        /// Deactivate a list of features
        /// </summary>
        /// <param name="service">Service object to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void DeactivateFeatures(this SPWebService service, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = service.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid existingFeature in existingFeatures.Intersect(featureIds))
            {
                service.Features.Remove(existingFeature);
            }
        }
    }
}
