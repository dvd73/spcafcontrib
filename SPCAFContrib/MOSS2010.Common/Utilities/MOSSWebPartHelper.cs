using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Common.Utilities;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace MOSS.Common.Utilities
{
    public class MOSSWebPartHelper : WebPartHelper
    {
        #region Singeton interface

        public static MOSSWebPartHelper Instance
        {
            get { return GetInstance<MOSSWebPartHelper>(); }
        }

        #endregion

        public static void ConnectWebPartOnPage(SPWeb oWeb, string strPageUrl,
            System.Web.UI.WebControls.WebParts.WebPart webPartConsumer,
            System.Web.UI.WebControls.WebParts.WebPart webPartProvider, string MappingId)
        {
            SPFile file = null;
            SPLimitedWebPartManager manager = null;
            file = oWeb.GetFile(strPageUrl);

            try
            {
                if (file.CheckOutType == SPFile.SPCheckOutType.None)
                {
                    file.CheckOut();
                    manager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

                    ConsumerConnectionPoint consumerConnection = null;
                    foreach (ConsumerConnectionPoint point in manager.GetConsumerConnectionPoints(webPartConsumer))
                    {
                        if (point.InterfaceType == typeof(IFilterValues))
                        {
                            consumerConnection = point; break;
                        }
                    }
                    ProviderConnectionPoint providerConnection = null;
                    foreach (ProviderConnectionPoint point in manager.GetProviderConnectionPoints(webPartProvider))
                    {
                        if (point.InterfaceType == typeof(Microsoft.SharePoint.WebPartPages.ITransformableFilterValues))
                        {
                            providerConnection = point; break;
                        }
                    }

                    TransformableFilterValuesToFilterValuesTransformer transformMapping = new TransformableFilterValuesToFilterValuesTransformer();
                    transformMapping.MappedConsumerParameterName = MappingId;
                    manager.SPConnectWebParts(webPartProvider, providerConnection, webPartConsumer, consumerConnection, transformMapping);

                    oWeb.Update();
                }
            }
            catch (Exception ex)
            {
                // exception here
            }
            finally
            {
                if (manager != null)
                    manager.Dispose();

                if (file != null && file.Exists)
                {
                    file.CheckIn("Connected web parts, if any");
                    file.Publish("Connected web parts, if any");
                }
            }

            // это код связывания двух веб частей. Не удалять, оставить для образца!
            //string providerConnectionId = "PagingData";
            //string consumerConnectionId = "PagingData";
            //if (objHomePageFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
            //    objHomePageFile.UndoCheckOut();

            //objHomePageFile.CheckOut();
            //bool wp_connected = false;

            //using (SPLimitedWebPartManager mgr = web.GetLimitedWebPartManager("Pages/news.aspx", PersonalizationScope.Shared))
            //{
            //    System.Web.UI.WebControls.WebParts.WebPart provider_wp = (from System.Web.UI.WebControls.WebParts.WebPart w in mgr.WebParts where w is ExtendedContentQueryWebPart select w).FirstOrDefault();
            //    System.Web.UI.WebControls.WebParts.WebPart consumer_wp = (from System.Web.UI.WebControls.WebParts.WebPart w in mgr.WebParts where w is ExtendedContentQueryWebPartPager select w).FirstOrDefault();

            //    if (provider_wp != null && consumer_wp != null)
            //    {
            //        ProviderConnectionPoint providerConnectionPoint = (from ProviderConnectionPoint conn in mgr.GetProviderConnectionPoints(provider_wp)
            //                                                           where conn.ID == providerConnectionId
            //                                                           select conn).FirstOrDefault();
            //        ConsumerConnectionPoint consumerConnectionPoint = (from ConsumerConnectionPoint conn in mgr.GetConsumerConnectionPoints(consumer_wp)
            //                                                           where conn.ID == consumerConnectionId
            //                                                           select conn).FirstOrDefault();

            //        SPWebPartConnection wp_conn = mgr.SPConnectWebParts(provider_wp, providerConnectionPoint, consumer_wp, consumerConnectionPoint);
            //        mgr.SPWebPartConnections.Add(wp_conn);
            //        wp_connected = true;
            //    }

            //    mgr.Web.Dispose();
            //}

            //if (wp_connected)
            //{
            //    objHomePageFile.CheckIn("Connect pagger");
            //    objHomePageFile.Publish("Connect pagger");
            //}
            //else
            //    objHomePageFile.UndoCheckOut();     
        }
    }
}
