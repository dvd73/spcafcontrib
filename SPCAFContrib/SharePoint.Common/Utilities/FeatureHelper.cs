using System;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.SharePoint.Administration;
using System.Xml;

namespace SharePoint.Common.Utilities
{
    public class FeatureHelper : SingletoneHelper
    {
        #region Singeton interface        

        public static FeatureHelper Instance
        {
            get { return GetInstance<FeatureHelper>(); }
        }

        #endregion                       

        public virtual SPWeb GetReceiverWeb(SPFeatureReceiverProperties properties)
        {
            object parent = properties.Feature.Parent;
            if (parent is SPWeb) return (SPWeb)parent;
            if (parent is SPSite) return ((SPSite)parent).RootWeb;
            throw new ApplicationException("Not supported feature scope.");
        }

        public virtual SPSite GetReceiverSite(SPFeatureReceiverProperties properties)
        {
            object parent = properties.Feature.Parent;
            if (parent is SPWeb) return ((SPWeb)parent).Site;
            if (parent is SPSite) return (SPSite)parent;
            throw new ApplicationException("Not supported feature scope.");
        }                
    }
}
