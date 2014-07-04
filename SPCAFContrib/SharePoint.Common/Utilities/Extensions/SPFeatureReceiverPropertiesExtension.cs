using System;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPFeatureReceiverPropertiesExtension
    {
        public static void Execute(this SPFeatureReceiverProperties properties,
                                    Action<SPFeatureReceiverProperties> action,
                                    Action<Exception> onException)
        {
            try
            {
                action(properties);
            }
            catch (Exception ex)
            {
                // TODO Lokitus

                if (onException == null ||
                    (properties.Feature.PropertyAsBool("Debug").HasValue && properties.Feature.PropertyAsBool("Debug").Value))
                    throw;
                else onException(ex);
            }

        }
    }
}
