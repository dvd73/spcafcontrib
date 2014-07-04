using System;
using Microsoft.SharePoint;
using SharePoint.Common.Utilities.Extensions;
using Microsoft.SharePoint.Utilities;

namespace SharePoint.Common.Utilities
{
    public class ListHelper : SingletoneHelper
    {
        #region Singeton interface
        
        public static ListHelper Instance
        {
            get { return GetInstance<ListHelper>(); }
        }

        #endregion   

        #region GetList and EnsureList

        public virtual SPList CreateList(string name, string template, SPWeb web)
        {
            // Ensure that list doesn't exist
            var listExists = web.Lists.TryGetList(name) != null;
            if (listExists) throw new SPException("List (name = {0} alreasy exists on web ({1}).".FormatWith(name, web.Url));

            // Create new list
            SPListTemplate listTemplate = GetListTemplate(web, template);
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            web.Lists.Add(name, "", listTemplate);
            web.Update();
            web.AllowUnsafeUpdates = allowUnsafeUpdates ;
            return web.Lists[name];
        }

        public virtual SPListTemplate GetListTemplate(SPWeb web, string name)
        {
            foreach (SPListTemplate l in web.ListTemplates)
            {
                if (string.Compare(l.Name, name, true /* ignoreCase */) == 0) return l;
            }
            foreach (SPListTemplate l in web.Site.GetCustomListTemplates(web))
            {
                if (string.Compare(l.Name, name, true /* ignoreCase */) == 0) return l;
            }

            Logger.Instance.LogWarning(string.Format("List template {0} not found.", name));
            return null;
        }
        #endregion

        #region ContentTypes

        public virtual void AddContentTypes(SPWeb web, string listTitle, string contentTypeList)
        {
            AddContentTypes(web, listTitle, contentTypeList.Split("#;"));
        }

        public virtual void AddContentTypes(SPWeb web, string listTitle, string[] contentTypeList)
        {
            web.ModifyList(listTitle, list =>
            {
                foreach (string contentTypeName in contentTypeList)
                {
                    list.EnsureContentType(web, contentTypeName);
                }
            });
        }

        #endregion

        public virtual void TryUsingListAtUrl(string webUrl, string listRelativeUrl, Action<SPList> action)
        {
            WebHelper.Instance.UsingWebAtUrl(webUrl, (SPWeb web) =>
            {
                try
                {
                    listRelativeUrl = web.SharePointUrlToRelativeUrl(listRelativeUrl);
                    var list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, listRelativeUrl));
                    if (list != null)
                        action(list);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            });
        }

        public virtual void TryUsingListAtUrlByTitle(string webUrl, string listTitle, Action<SPList> action)
        {
            WebHelper.Instance.UsingWebAtUrl(webUrl, (SPWeb web) =>
            {
                var list = web.Lists.TryGetList(listTitle);
                if (list != null)
                    action(list);
            });
        }

    }
}
