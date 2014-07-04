using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPWebExtension
    {
        /// <summary>
        /// <para>Returns a list containing the ServerRelativeUrls of each SPWeb in web.Webs</para>
        /// <para>Wrapped SPWebs are properly Disposed</para>
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static List<string> GetSubWebUrls(this SPWeb web)
        {
            var subWebUrls = new List<string>();

            int subWebCount = web.Webs.Count;

            for (int i = 0; i < subWebCount; i++)
                using (var subWeb = web.Webs[i])
                    subWebUrls.Add(subWeb.ServerRelativeUrl);

            return subWebUrls;
        }        

        public static string GetWelcomePageUrl(this SPWeb web)
        {
            return SPUtility.ConcatUrls(web.Url, web.RootFolder.WelcomePage);
        }

        /// <summary>
        /// Activate a list of features
        /// </summary>
        /// <param name="web">Web to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void ActivateFeatures(this SPWeb web, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = web.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid featureId in featureIds.Except(existingFeatures))
            {
                web.Features.Add(featureId);
            }
        }

        /// <summary>
        /// Deactivate a list of features
        /// </summary>
        /// <param name="web">Web object to activate the features on</param>
        /// <param name="featureIds">List of feature ID's to activate</param>
        public static void DeactivateFeatures(this SPWeb web, IEnumerable<Guid> featureIds)
        {
            IEnumerable<Guid> existingFeatures = web.Features.Select<SPFeature, Guid>(f => f.DefinitionId);
            foreach (Guid existingFeature in existingFeatures.Intersect(featureIds))
            {
                web.Features.Remove(existingFeature);
            }
        }

        /// <summary>
        /// Try to get a list, if list is not found try to create it.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="templateName"></param>
        /// <param name="initializeNewList"></param>
        /// <returns></returns>
        public static SPList EnsureList(this SPWeb web, string listName, string templateName, Action<SPList> initializeNewList)
        {
            var list = web.Lists.TryGetList(listName);
            if (list == null)
            {
                var template = web.ListTemplates[templateName];
                var listGuid = web.Lists.Add(listName, null, template);
                var newList = web.Lists[listGuid];
                initializeNewList(newList);
                newList.Update();
                list = newList;
            }
            return list;
        }

        public static SPList EnsureList(this SPWeb web, string listName, string template)
        {
            var list = web.Lists.TryGetList(listName.Trim());
            if (list == null)
            {
                if (listName != null && template != null)
                {
                    list = ListHelper.Instance.CreateList(listName, template, web);
                }
            }

            // if not created or found
            if (list == null)
                throw new Exception(listName + "-library not found.");

            return list;
        }

        public static void TryUsingListByTitle(this SPWeb web, string listTitle, Action<SPList> action)
        {
            try
            {
                if (web.Lists.Count > 0)
                {
                    SPList list = web.Lists.Cast<SPList>().Where(l => string.Equals(l.Title, listTitle.Trim(), StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
                    if (list != null)
                        action(list);
                }
            }
            catch (Exception ex)
            {
                ex.LogError(String.Format("List URL: {0}<br/>Action: <br/>{1}<br/>{2}", listTitle, SPHttpUtility.HtmlEncode(action.Method.DeclaringType.FullName), SPHttpUtility.HtmlEncode(action.Method.Name)));
            }
        }

        public static void TryUsingList(this SPWeb web, string listRelativeUrl, Action<SPList> action)
        {           
            try
            {
                listRelativeUrl = web.SharePointUrlToRelativeUrl(listRelativeUrl);

                if (web.Lists.Count > 0)
                {
                    SPList list = web.GetList(listRelativeUrl);
                    if (list != null)
                        action(list);
                }
            }
            catch (Exception ex)
            {
                ex.LogError(String.Format("List URL: {0}<br/>Action: <br/>{1}<br/>{2}", listRelativeUrl, SPHttpUtility.HtmlEncode(action.Method.DeclaringType.FullName), SPHttpUtility.HtmlEncode(action.Method.Name)));
            }
        }        

        public static void TryUsingListItem(this SPWeb web, string listTitle, Action<SPListItem> action)
        {
            web.TryUsingListByTitle(listTitle, list =>
            {
                foreach (SPListItem item in list.Items)
                {
                    try
                    {
                        action(item);
                    }
                    catch (Exception ex)
                    {
                        ex.LogError((String.Format("List title: {0}<br/>List item: {1}<br/>Action: <br/>{2}<br/>{3}", listTitle, item["Title"].ToString(), SPHttpUtility.HtmlEncode(action.Method.DeclaringType.FullName), SPHttpUtility.HtmlEncode(action.Method.Name))));
                    }
                }
            });
        }

        public static void RemoveContentTypes(this SPWeb web, string listName, string contentTypeList)
        {
            RemoveContentTypes(web, listName, contentTypeList.Split("#;"));
        }

        public static void RemoveContentTypes(this SPWeb web, string listName, string[] contentTypeList)
        {
            web.ModifyList(listName, list =>
            {
                foreach (string contentTypeName in contentTypeList)
                {
                    list.RemoveContentType(contentTypeName);
                }
            });
        }

        /// <summary>
        /// Try to get settign list, ensure also, that setting list contains all mentioned fields. If a field is missing automatically crates one.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="textFieldsToAdd"></param>
        /// <returns></returns>
        public static SPList EnsureSettingsListWithFields(this SPWeb web, string listName, List<string> textFieldsToAdd)
        {
            var list = web.EnsureList(listName, "Custom List in Datasheet View",
                                  listToInitialize =>
                                  {
                                      foreach (var fieldName in textFieldsToAdd)
                                      {
                                          listToInitialize.Fields.Add(fieldName, SPFieldType.Text, false);
                                      }
                                  });
            // ensure fields
            bool hasListUpdated = false;
            foreach (string fieldName in textFieldsToAdd)
            {

                if (!list.Fields.ContainsField(fieldName))
                {
                    list.Fields.Add(fieldName, SPFieldType.Text, false);
                    hasListUpdated = true;
                }
            }
            if (hasListUpdated) list.Update();
            return list;
        }

        public static void ModifyList(this SPWeb web, string listTitle, Action<SPList> modification)
        {
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            web.TryUsingListByTitle(listTitle, list =>
            {
                modification(list);
                list.Update();
            });
            web.Update();
            web.AllowUnsafeUpdates = allowUnsafeUpdates ;
        }
    }
}
