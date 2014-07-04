using SharePoint.Common.Utilities;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint;
using CamlexNET;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using System;
using System.Xml;

namespace MOSS.Common.Utilities
{
    public class MOSSFeatureHelper : FeatureHelper
    {
        public new static MOSSFeatureHelper Instance
        {
            get { return GetInstance<MOSSFeatureHelper>(); }
        }

        public void RemoveProvisionedWebParts(SPWeb rootweb, SPFeatureReceiverProperties properties)
        {
            // Find the Web Part names from the Elements collection
            List<string> webparts = new List<string>();
            SPElementDefinitionCollection elementColletion = properties.Definition.GetElementDefinitions(CultureInfo.CurrentCulture);
            foreach (SPElementDefinition element in elementColletion)
            {
                foreach (XmlElement xmlNode in element.XmlDefinition.ChildNodes)
                {
                    if (xmlNode.Name.Equals("File"))
                    {
                        webparts.Add(xmlNode.Attributes["Url"].Value);
                    }
                }
            }

            // Get the Web Part Catalog
            SPList wpGallery = rootweb.GetCatalog(SPListTemplateType.WebPartCatalog);

            // Find the list items that matchs the Feature Web Parts
            List<SPListItem> items = new List<SPListItem>();
            foreach (SPListItem item in wpGallery.Items)
            {
                if (webparts.Contains(item.File.Name))
                {
                    items.Add(item);
                }
            }

            // Remove the Feature Web Parts from the Web Part catalog
            foreach (SPListItem item in items)
            {
                item.Delete();
            }
        }

        public void RemoveProvisionedPages(SPWeb web, SPFeatureReceiverProperties properties)
        {
            SPSite site = web.Site;

            // Removes the Feature Web Parts from the Web Part Catalog on the site collection. 

            // Find the Web Part names from the Elements collection
            List<string> pages = new List<string>();
            SPElementDefinitionCollection elementColletion = properties.Definition.GetElementDefinitions(CultureInfo.CurrentCulture);
            foreach (SPElementDefinition element in elementColletion)
            {
                foreach (XmlElement xmlNode in element.XmlDefinition.ChildNodes)
                {
                    if (xmlNode.Name.Equals("File"))
                    {
                        pages.Add(xmlNode.Attributes["Url"].Value);
                    }
                }
            }

            if (pages.Count > 0)
            {
                // Get the Web Part Catalog
                PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(web);
                SPList pagesList = publishingWeb.PagesList;

                SPQuery query = new SPQuery();
                var expressions = new List<Expression<Func<SPListItem, bool>>>();
                foreach (string pageUrl in pages)
                {
                    string p = pageUrl;
                    expressions.Add(x => ((string)x["FileLeafRef"]).Contains(p));
                }
                query.Query = Camlex.Query().WhereAny(expressions).ToString(); ; // Camlex.Query().Where(x => ((string)x["FileLeafRef"]).Contains(".aspx")).ToString();
                query.ViewAttributes = "Scope='RecursiveAll'";
                SPListItemCollection listItems = pagesList.GetItems(query);
                web.AllowUnsafeUpdates = true;
                for (int i = listItems.Count - 1; i >= 0; i--)
                    listItems[i].Delete();
                pagesList.Update();
                web.AllowUnsafeUpdates = false;
            }
        }
    }
}
