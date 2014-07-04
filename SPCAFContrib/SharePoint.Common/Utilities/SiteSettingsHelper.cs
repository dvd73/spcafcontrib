using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities
{
    public class SiteSettingsHelper
    {
        private static object lock_object = new object();

        public static string GetStringValue(SPWeb web, string key, string default_value)
        {
            return SiteSettingsHelper.HandleValue<string>(web, key, (item) =>
            {
                string result = StringHelper.CheckForNull(item[item.Fields.GetFieldByInternalName("Categories").Id]);
                if (String.IsNullOrEmpty(result))
                    result = default_value;

                return result;
            });
        }

        public static int GetIntValue(SPWeb web, string key, int default_value)
        {
            return SiteSettingsHelper.HandleValue<int>(web, key, (item) =>
            {
                int result = -1;
                object value = item[item.Fields.GetFieldByInternalName("Value").Id];
                if (value == null) value = default_value;

                if (!Int32.TryParse(value.ToString(), out result))
                    result = default_value;

                return result;
            });
        }

        public static double GetDoubleValue(SPWeb web, string key, double default_value)
        {
            return SiteSettingsHelper.HandleValue<double>(web, key, (item) =>
            {
                double result = -1.0;
                object value = item[item.Fields.GetFieldByInternalName("Value").Id];
                if (value == null) value = default_value;

                if (!Double.TryParse(value.ToString(), out result))
                    result = default_value;

                return result;
            });
        }

        public static T HandleValue<T>(SPWeb web, string key, Func<SPListItem, T> proc)
        {
            T result = default(T);

            HandleGettingValue(web, key, (item) =>
            {
                result = proc(item);
            });

            return result;
        }

        private static void HandleGettingValue(SPWeb web, string key, Action<SPListItem> proc)
        {
            SPList list = web.Lists.TryGetList(Consts.SITE_SETTINGS_LIST_NAME);
            if (list != null)
            {
                SPQuery qry = new SPQuery();
                qry.RowLimit = 1;
                qry.Query = String.Format(
                @"   <Where>
                        <Eq>
                            <FieldRef Name='Title' />
                            <Value Type='Text'>{0}</Value>
                        </Eq>
                    </Where>", key);
                SPListItemCollection listItems = list.GetItems(qry);

                SPListItem item = null;
                if (listItems.Count == 0)
                {
                    bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;

                    item = list.AddItem();
                    item[item.Fields.GetFieldByInternalName("Title").Id] = key;
                    item.Update();

                    web.AllowUnsafeUpdates = allowUnsafeUpdates;
                }
                else
                    item = listItems[0];

                lock (lock_object)
                {
                    proc(item);                   
                }
            }
        }
    }
}
