using System;
using System.Linq;
using System.Data;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SharePoint.Common.Utilities.Extensions;

namespace SharePoint.Common.Utilities
{
    /// <summary>
    /// This class contains properties and method for getting Carousel feed listitems.
    /// </summary>
    public class Carousel
    {
        /// <summary>
        /// List url that carouselfeed items are queried
        /// </summary>
        public static string ListUrl { get; set; }

        /// <summary>
        /// Comma separated list of fields (columns) that are retrieved from list
        /// </summary>
        public static string DisplayFields { get; set; }

        /// <summary>
        /// Field that is used for sorting carousel feeditem
        /// </summary>
        public static string SortField { get; set; }

        /// <summary>
        /// Field that contains information whether carousel feeditem is active or not
        /// </summary>
        public static string WhereField { get; set; }

        /// <summary>
        /// Information whether cache is being used
        /// </summary>
        public static bool UseCache { get; set; }

        /// <summary>
        /// Cache key (if cache is used)
        /// </summary>
        public static string CacheKey { get; set; }

        /// <summary>
        /// Gets all Carouselfeed items from list that is defined in class property.
        /// </summary>
        /// <returns>DataTable</returns>
        public static DataTable GetCarouselFeedItems()
        {
            try
            { 
                if(string.IsNullOrEmpty(ListUrl) || string.IsNullOrEmpty(DisplayFields)) {
                    throw new Exception("Carousel ListUrl and/or DisplayFields properties are not defined.");
                }

                if (!SPUrlUtility.IsUrlRelative(ListUrl))
                {
                    ListUrl = UrlHelper.SharePointUrlToRelativeUrl(ListUrl);
                }

                DataTable dt = null;
                using (SPWeb web = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ID))
                {
                    web.TryUsingList(ListUrl, carouselList =>
                    {
                        string viewFields = viewFieldsFromStringList(DisplayFields);

                        string query = string.Empty;
                        if (!string.IsNullOrEmpty(WhereField))
                        {
                            query = "<Where><Eq><FieldRef Name='" + WhereField + "' /><Value Type='bit'>1</Value></Eq></Where>";
                        }
                        if (!string.IsNullOrEmpty(SortField))
                        {
                            query = query + "<OrderBy><FieldRef Name='" + SortField + "' Ascending='TRUE' /></OrderBy>";
                        }
                        SPListItemCollection items = carouselList.GetItems(0, viewFields, query);
                        dt = items.GetDataTable();

                        if (UseCache)
                        {
                            CacheDispatcher.Current.Add(CacheKey, dt);
                        }
                    });
                }

                return dt;
            }
            catch(Exception ex)
            {
                ex.LogError();
                return null;
            }

        }

        private static string viewFieldsFromStringList(string list)
        {
            try
            {
                if (!string.IsNullOrEmpty(list))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string field in list.Split(','))
                    {
                        sb.Append("<FieldRef Name='" + field + "'/>");
                    }
                    return sb.ToString();
                }
                else
                {
                    throw new Exception("Carousel field-list is empty.");
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
                return string.Empty;
            }

        }

        public static string getUrl(object linkText)
        {
            if (linkText != null)
            {
                if (!string.IsNullOrEmpty(linkText.ToString()))
                {
                    return linkText.ToString().Substring(0, linkText.ToString().IndexOf(","));
                }
                else
                {
                    return string.Empty;
                }
            }
            else { return string.Empty; }
        }

        public static string getLinkText(object linkText)
        {
            if (linkText != null)
            {
                if (!string.IsNullOrEmpty(linkText.ToString()))
                {
                    return linkText.ToString().Substring((linkText.ToString().IndexOf(",") + 1));
                }
                else
                {
                    return string.Empty;
                }
            }
            else { return string.Empty; }
        }

        public static bool showField(object linkText)
        {
            if (linkText != null)
            {
                if (!string.IsNullOrEmpty(linkText.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else { return false; }
        }
    }
}
