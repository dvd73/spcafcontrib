using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPListExtension
    {
        #region Linq
        public static IEnumerable<T> Select<T>(this SPListItemCollection items, Func<SPListItem, T> selector)
        {
            foreach (SPListItem item in items)
                yield return selector(item);
        }

        public static IEnumerable<T> SelectIf<T>(this SPListItemCollection items, Func<SPListItem, bool> wherePredicate, Func<SPListItem, T> selector)
        {
            foreach (SPListItem item in items)
                if (wherePredicate(item))
                    yield return selector(item);
        }

        public static IEnumerable<SPListItem> WhereEx(this SPListItemCollection items,
                                            Func<SPListItem, bool> predicate)
        {
            foreach (SPListItem item in items)
                if (predicate(item))
                    yield return item;
        } 
        #endregion

        #region ContentType
        /// <summary>
        /// Ensure that list contain particular content type. Note! Do not call update() to save changes to the list
        /// </summary>
        /// 
        /// <param name="list"></param>
        /// <param name="web"></param>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public static bool EnsureContentType(this SPList list, SPWeb web, string contentTypeName)
        {
            if (web.Site.RootWeb.ContentTypes[contentTypeName] == null)
                return false;
            else
            {
                SPContentType ct = web.Site.RootWeb.ContentTypes[contentTypeName];
                if (list.ContentTypes[contentTypeName] == null)
                {
                    list.ContentTypes.Add(ct);
                }
                return true;
            }
        }

        public static bool RemoveContentType(this SPList list, string contentTypeName)
        {
            if (list == null) throw new NullReferenceException("SPList given as parameter is null.");
            if (contentTypeName == null) throw new NullReferenceException("ContentTypeName given as parameter is null.");

            SPContentType contentType = null;

            try
            {
                if (Regex.IsMatch(contentTypeName, "^0x", RegexOptions.IgnoreCase))
                    contentType = list.ContentTypes[list.ContentTypes.BestMatch(new SPContentTypeId(contentTypeName))];
                else
                    contentType = list.ContentTypes[contentTypeName];
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            if (contentType != null)
            {
                // Default content type cannot be removed; rearrange them before removing
                //if (list.ContentTypes[0] == contentType && list.ContentTypes.Count > 1)
                //{
                //    var contentTypes = list.ContentTypes.Cast<SPContentType>().Skip(1).ToList();
                //    contentTypes.Insert(1, contentType);
                //    list.RootFolder.UniqueContentTypeOrder = contentTypes;
                //    list.RootFolder.Update();
                //}

                list.ContentTypes.Delete(contentType.Id);
                return true;
            }
            return false;
        } 
        #endregion

        #region DocumentLibrary Upload

        /// <summary>
        /// Upload files to a list's root folder and check them in if possible
        /// </summary>
        /// <param name="list"></param>
        /// <param name="files">String filename, string contenttype. E.g. 
        ///   <parameter key="c:/filepath/dokument.docx">contentypename</add>
        /// </param>
        /// <returns></returns>
        public static bool UploadFiles(SPList list, Dictionary<string, string> files)
        {
            if (list == null || files == null) return false;
            foreach (KeyValuePair<string, string> file in files)
            {
                // if key is same than a existing file
                if (File.Exists(file.Key))
                {
                    Hashtable fileProperties = new Hashtable();

                    string ctname = file.Value;
                    SPContentType ct = list.ContentTypes[ctname];

                    if (ct != null)
                        fileProperties["ContentTypeId"] = ct.Id.ToString();

                    UploadFile(list, file.Key, fileProperties);
                }
            }
            return true;
        }

        public static bool UploadFile(this SPList list, string filePath, Hashtable fileProperties)
        {
            if (list == null || string.IsNullOrEmpty(filePath)) return false;
            if (File.Exists(filePath))
            {
                if (fileProperties == null)
                    fileProperties = new Hashtable();

                SPFile uploadedfile = list.RootFolder.Files.Add(
                    Path.GetFileName(filePath),
                    File.ReadAllBytes(filePath),
                    fileProperties
                    );
                try
                {
                    if (fileProperties["ContentTypeId"] != null)
                    {
                        // need to update SPFile.Item properties
                        // Hashtable fileproperties only works for certain file types
                        // for example, .xml files get wrong Content Type if Item properties are not set
                        uploadedfile.Item["ContentTypeId"] = fileProperties["ContentTypeId"];
                        uploadedfile.Item["ContentType"] = list.ContentTypes[new SPContentTypeId((string)fileProperties["ContentTypeId"])];
                        uploadedfile.Item.Update();
                    }
                    uploadedfile.CheckIn("", SPCheckinType.MajorCheckIn);
                }
                catch { }
            }
            list.Update();
            return true;
        }
        
        public static string UploadBytes(this SPList list,
                                      byte[] file,
                                      string fileName,
                                      string folderName,
                                      Action<SPFile> setProperties)
        {
            if (list == null || file == null || file.Length < 1) return null;
            SPFolder folder = list.GetFolder(folderName);
            SPFile uploadedfile = folder.Files.Add(fileName, file, true, "", false);
            if (setProperties != null) setProperties(uploadedfile);
            uploadedfile.Update();
            // uploadedfile.
            list.Update();
            return (list.ParentWebUrl + "/" + uploadedfile.Item.Url).Replace("//", "/");
        }

        #endregion
        
        #region View
        public static SPList AddFieldsToView(this SPList list, SPWeb web, string fields)
        {
            list.AddFieldsToView(web, fields.Split("#;"));
            return list;
        }

        public static SPList AddFieldsToView(this SPList list, SPWeb web, string[] fields)
        {
            SPView view = list.Views[0];
            list.AddFieldsToView(view, web, fields);
            return list;
        }

        public static SPList AddFieldsToView(this SPList list, SPWeb web, string viewName, string[] fields)
        {
            SPView view = list.Views[viewName];
            list.AddFieldsToView(view, web, fields);
            return list;
        }

        public static SPView AddFieldsToView(this SPList list, SPView view, SPWeb web, string[] fields)
        {
            view.Update(web,
                   v =>
                   {
                       foreach (string listItem in fields)
                       {
                           view.ViewFields.Add(listItem);
                       }
                   });
            return view;
        }

        public static SPList RemoveFieldsFromView(this SPList list, SPWeb web, string viewName, string[] fields)
        {
            list.ModifyView(viewName, view =>
            {
                foreach (string listItem in fields)
                {
                    if (!view.ViewFields.Exists(listItem))
                        view.ViewFields.Delete(listItem);
                }
            });
            return list;
        }

        public static void ModifyView(this SPList list, string viewName, Action<SPView> action)
        {
            SPView view = list.Views[viewName];
            action(view);
            view.Update();
        }

        #endregion

        #region Insert
        public static SPListItem InsertItem(this SPList list, Action<SPListItem> updateItemAction)
        {
            SPListItem item = list.AddItem();
            updateItemAction(item);
            item.Update();
            return item;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete each item, that fullfill where-condition (passed as parameter deleteWherePredicate)
        /// </summary>
        /// <param name="list">List where to delete items.</param>
        /// <param name="condition">Specify what items will be deleted.</param>
        public static void DeleteItems(this SPList list, Func<SPListItem, bool> condition)
        {
            // Collection is modified thus itereate using for-loop starting form last item.
            // Foreach won't work!
            int lastItemIndex = list.ItemCount - 1;
            for (int i = lastItemIndex; i >= 0; i--)
            {
                SPListItem item = list.Items[i];
                if (condition(item))
                {
                    item.Delete();
                }
            }
        }       

        #endregion

        #region SiteColumn

        public static SPField AddSiteColumn(this SPList list, string name)
        {
            SPField field = null;
            WebHelper.Instance.UsingCurrentWeb(web =>
            {
                field = AddSiteColumn(list, web, name);
            });
            return field;
        }

        public static SPField AddSiteColumn(this SPList list, SPWeb web, string name)
        {
            SPField field = web.AvailableFields.GetField(name);
            list.Fields.Add(field);
            return field;
        }
        #endregion

        #region CAML

        /// <summary>
        /// Gets the items by specified query.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="whereCondition">The query.</param>
        ///     E.g. <Eq><FieldRef Name='Author' LookupId='TRUE' /><Value Type='User'><UserID /></Value></Eq>
        ///     Exclude where.
        /// <returns></returns>
        public static SPListItemCollection GetItems(this SPList list, string whereCondition)
        {
            return list.GetItems(null, 0, null, whereCondition, String.Empty);
        }

        /// <summary>
        /// Gets the items by speicified query. The number of rows is limited by specified row limit.
        /// Returned list will contains the fields that are specified as comma separated list in viewFields parameter.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="rowLimit">The row limit.</param>
        /// <param name="viewFields">The view fields.</param>
        /// <param name="whereCondition">The query.</param>
        ///      E.g. <Eq><FieldRef Name='Author' LookupId='TRUE' /><Value Type='User'><UserID /></Value></Eq>
        ///     Exclude where.
        /// <returns></returns>
        public static SPListItemCollection GetItems(this SPList list, int rowLimit, string viewFields, string whereCondition)
        {
            return list.GetItems(null, rowLimit, viewFields, whereCondition, String.Empty);
        }

        /// <summary>
        /// Gets the items by speicified query and folder. The number of rows is limited by specified row limit.
        /// Returned list will contains the fields that are specified as comma separated list in viewFields parameter.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="rowLimit">The row limit.</param>
        /// <param name="viewFields">The view fields.</param>
        /// <param name="whereCondition">The Where query.</param>
        ///     E.g. <Eq><FieldRef Name='Author' LookupId='TRUE' /><Value Type='User'><UserID /></Value></Eq>
        ///     Exclude where.
        /// <param name="orderByCondition">The query for order rule.</param>
        /// <returns></returns>
        public static SPListItemCollection GetItems(this SPList list, SPFolder folder, int rowLimit, string viewFields, string whereCondition, string orderByCondition)
        {
            SPQuery q = new SPQuery();

            q.Folder = folder;
            q.RowLimit = Convert.ToUInt32(rowLimit);
            q.ViewFields = viewFields;

            if (whereCondition.StartsWith("<Where>") && orderByCondition.StartsWith("<OrderBy>"))
                q.Query = whereCondition + orderByCondition;
            else
            {
                q.Query = !String.IsNullOrEmpty(whereCondition) ? @"<Where>{0}</Where>".FormatWith(whereCondition) : String.Empty;
                q.Query += !String.IsNullOrEmpty(orderByCondition) ? @"<OrderBy>{1}</OrderBy>".FormatWith(orderByCondition) : String.Empty;            
            }            

            q.Query = whereCondition;

            return list.GetItems(q);
        }         

        public static IEnumerable<SPListItem> Find(this SPList list, Func<SPListItem, bool> wherePredicate)
        {
            return (from SPListItem item in list.Items where wherePredicate(item) select item);
        }

        #endregion

        #region Folder
        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        public static SPFolder GetFolder2(this SPList list, string folderName)
        {
            string url = list.ParentWeb.Url + "/" + list.Title + "/" + folderName;

            return list.ParentWeb.GetFolder(url);
        }

        public static SPFolder GetFolder(this SPList list, string folderName)
        {
            if (folderName.IsNullOrEmptyAfterTrim() || folderName == "/") return list.RootFolder;
            return (from SPFolder folder in list.Folders where folder.Name == folderName select folder).SingleOrDefault();
        }

        public static SPFolder EnsureFolder(this SPList list, string folderName)
        {
            var folder = list.GetFolder(folderName);
            if (folder == null)
            {
                string serverRelatativeUrl = list.RootFolder.Url + folderName;
                list.Folders.Add(serverRelatativeUrl, SPFileSystemObjectType.Folder);
            }
            return list.GetFolder(folderName);
        } 
        #endregion

        public static SPList Update(this SPList list, SPWeb web, Action<SPList> action)
        {
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            action(list);
            list.Update();
            web.Update();
            web.AllowUnsafeUpdates = allowUnsafeUpdates;
            return list;
        }

        public static bool TryGetValueAsString(this SPListItem item, string fieldName, out string value)
        {
            value = null;
            if (item != null && item.Fields.ContainsField(fieldName))
            {
                var objValue = item[fieldName];
                if (objValue != null)
                {
                    value = objValue.ToString();
                    return !String.IsNullOrEmpty(value);
                }
            }
            return false;
        }

        public static string GetValueAsString(this SPListItem item, string fieldName)
        {
            string value = null;
            item.TryGetValueAsString(fieldName, out value);
            return value;
        }

        public static bool TryGetValueAsInt(this SPListItem item, string fieldName, out int result)
        {
            string value;
            result = 0;
            return item.TryGetValueAsString(fieldName, out value) && int.TryParse(value, out result);
        }

        public static bool TryGetValueAsBool(this SPListItem item, string fieldName, out bool result)
        {
            string value;
            result = false;
            return item.TryGetValueAsString(fieldName, out value) && bool.TryParse(value, out result);
        }

        public static bool TryParseLinkToInt(this SPListItem item, string fieldName, out int result)
        {
            string value;
            result = 0;
            if (item.TryGetValueAsString(fieldName, out value) && value.IndexOf(";") > 0)
            {
                return int.TryParse(value.Substring(0, value.IndexOf(";")), out result);
            }
            return false;
        }
    }
}
