using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPListItemExtension
    {
        private static readonly string AccountFieldId = "Account"; // new Guid("bfc6f32c-668c-43c4-a903-847cca2f9b3c");


        public static string GetFieldValueUserLogin(this SPListItem item, Guid fieldId)
        {
            if (item != null)
            {
                SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[fieldId] as string);
                return userValue.User.LoginName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the login name of an User-Field.
        /// </summary>
        public static SPUser GetFieldUser(this SPListItem item, string fieldName)
        {
            if (item != null)
            {
                SPFieldUserValue userValue =
                  new SPFieldUserValue(
                    item.Web, item[fieldName] as string);
                return userValue.User;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets field value. Does not do anything if field is not found.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to set</param>
        /// <param name="value"></param>
        public static void TrySetFieldValue(this SPListItem item, string name, object value)
        {
            var field = item.TryGetField(name);
            if (field != null)
                item[field.StaticName] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        public static bool TrySetLookUpFieldValue(this SPListItem item, string name, Func<SPListItem, bool> predicate, bool throwOnError)
        {
            var field = item.TryGetField("Name");
            throw new NotImplementedException();
        }

        [Obsolete("Untested. Remove this attribute when you know this works.", false)]
        public static bool TrySetUserFieldValue(this SPListItem item, string fieldName, string accounts, bool throwOnError)
        {
            return TrySetUserFieldValue(item, fieldName, new List<string>() { accounts }, throwOnError);
        }

        /// <summary>
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <param name="fieldName">Field name (of type LookUpMulti)</param>
        /// <param name="values">List of id values form the lookup target list. </param>
        /// <param name="throwOnError">True: throw Exceptions on errors, False: fail silently.</param>
        /// <returns>True if setting fiels values succeeded, false if it did not.</returns>
        [Obsolete("Untested. Remove this attribute when you know this works.", false)]
        public static bool TrySetUserFieldValue(this SPListItem item, string fieldName, List<string> accounts, bool throwOnError)
        {
            try
            {
                var field = item.TryGetField(fieldName);
                if (field == null)
                {
                    if (throwOnError)
                        new ArgumentException(string.Format("Field {0} does not exist.", fieldName));
                    return false;
                }
                var userField = field as SPFieldUser;
                if (userField == null)
                {
                    if (throwOnError)
                    {
                        var formatArguments = new string[] { fieldName, typeof(Microsoft.SharePoint.SPFieldUser).Name, field.GetType().Name };
                        throw new InvalidCastException(
                            string.Format("Cannot cast value of field '{0}'. Type expected: {1} . Field value type: {2} .", formatArguments));
                    }
                    return false;
                }
                var webId = userField.LookupWebId;
                var listId = userField.LookupList;
                List<SPFieldUserValue> userFieldValues = GetSPFieldsUserValuesFromUserList(item, webId, listId, accounts);

                if (userFieldValues.Count == 0)
                {
                    item[field.StaticName] = null;
                }
                else if (userField.AllowMultipleValues)  // field is of the expected type
                {
                    var userValueCollection = new SPFieldUserValueCollection();
                    userValueCollection.AddRange(userFieldValues);
                    item[field.StaticName] = userValueCollection;
                }
                else
                {
                    item[field.StaticName] = userFieldValues.FirstOrDefault();
                }
                return true;

            }
            catch (Exception ex)
            {
                ex.LogError();

                if (throwOnError)
                    throw;

                return false;
            }
        }

        private static List<SPFieldUserValue> GetSPFieldsUserValuesFromUserList(this SPListItem item, Guid webId, string listIdString, IEnumerable<string> accounts)
        {
            List<SPFieldUserValue> userFieldValues;
            using (var site = new SPSite(item.Web.Site.ID))
            using (var web = site.OpenWeb(webId))
            {
                Guid listId = new Guid(listIdString);
                var list = web.Lists[listId];
                userFieldValues = list.Items.SelectIf(
                    userRow =>
                    {
                        var account = userRow.TryGetField(AccountFieldId) == null
                                          ? null
                                          : userRow.TryGetFieldValue(AccountFieldId).ToString();
                        return accounts.Any(a => String.Equals(a, account, StringComparison.InvariantCultureIgnoreCase));
                    },
                    userRow => new SPFieldUserValue(item.Web, userRow.ID, userRow[AccountFieldId].ToString()))
                                             .ToList();
            }
            return userFieldValues;
        }

        /// <summary>
        /// <para>Set lookup value. Notice you still need to call item.Update().</para>
        /// <para>Returns true if setting field values succeeded, otherwise false.</para>
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <param name="fieldName">Field name (of type LookUpMulti)</param>
        /// <param name="values">List of id values form the lookup target list. </param>'
        /// <param name="throwOnError">True: throw Exceptions on errors, False: fail silently.</param>
        /// <returns>True if setting fiels values succeeded, false if it did not.</returns>
        [Obsolete("Untested. Remove this attribute when you know this works.", false)]
        public static bool TrySetLookUpMultiFieldValue(this SPListItem item, string fieldName, List<int> values, bool throwOnError)
        {
            try
            {
                var field = item.TryGetFieldValue(fieldName);
                if (field != null) // field exists
                {

                    var fieldValues = field as Microsoft.SharePoint.SPFieldLookupValueCollection;
                    if (fieldValues != null)  // field is of the expected type
                    {
                        fieldValues.Clear();
                        values.ForEach(id => fieldValues.Add(new SPFieldLookupValue(id, null)));
                        item.TrySetFieldValue(fieldName, fieldValues);
                        return true;
                    }
                    else
                    {
                        if (throwOnError)
                        {
                            var formatArguments = new string[] { fieldName, typeof(Microsoft.SharePoint.SPFieldLookupValueCollection).Name, fieldValues.GetType().Name };
                            throw new InvalidCastException(
                                string.Format("Cannot cast value of field '{0}'. Type expected: {1} . Field value type: {2} .", formatArguments));
                        }
                        return false;
                    }
                }
                else
                {
                    if (throwOnError)
                        new ArgumentException(string.Format("Field {0} does not exist.", fieldName));

                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.LogError();

                if (throwOnError)
                    throw;

                return false;
            }
        }

        /// <summary>
        /// Set lookup value. Should not throw exception in anycase. Should not do anything if fails.
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <param name="fieldName">Field name (of type LookUpMulti)</param>
        /// <param name="values">List of id values form the lookup target list. </param>'
        /// <returns>True if setting fiels values succeeded, false if it did not.</returns>
        public static bool TrySetLookUpMultiFieldValue(this SPListItem item, string fieldName, List<int> values)
        {
            return TrySetLookUpMultiFieldValue(item, fieldName, values, false);
        }

        /// <summary>
        /// Gets field value. Does not do anything if field is not found. Notice you still need to call item.Update().
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to set</param>
        /// <param name="value"></param>
        public static List<T> TryGetLookUpFieldValues<T>(this SPListItem item, string name, Func<SPListItem, T> mappingBetweenList)
        {

            var field = item.TryGetField(name);
            if (field != null)
            {
                var retVal = new List<T>();
                var originalVal = item[field.StaticName];
                object val;
                if (originalVal is string)
                {
                    val = field.GetFieldValue(originalVal.ToString());
                }

                // Return null
            }
            return null;
            //Microsoft.SharePoint.SPFieldLookup
        }

        /// <summary>
        /// Gets field. Returns null if field is not found.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to get</param>
        /// <returns></returns>
        public static SPField TryGetField(this SPListItem item, string name)
        {
            SPField field = null;
            if (item.Fields.ContainsField(name))
                field = item.Fields.GetField(name);
            return field;
        }

        /// <summary>
        /// Gets field. Returns string.Empty if resulting value is null.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to get</param>
        /// <returns></returns>
        public static string TryGetFieldValueAsString(this SPListItem item, string name)
        {
            return (item.TryGetFieldValue(name) ?? string.Empty).ToString();
        }

        /// <summary>
        /// Gets field value of values and wrap them in List&lt;string&gt; that containsa values of any field.
        /// 
        /// If field does not allow multible values, then the list contains only one item. If it does, it contains all items as string.
        /// </summary>
        /// <typeparam name="TFieldType">Field value type: E.g. SPFieldUserValue, object, SPFieldLookUpValue</typeparam>
        /// <param name="item">SPListItem</param>
        /// <param name="name">SPField.Name</param>
        /// <param name="getValue">Func that specify how value in list item is transformed into string. Notice will be ignored if field type is MultiChoiseField!</param>
        /// <param name="throwExeption"></param>
        /// <returns></returns>
        public static List<string> GetFieldValueAsStringList<TFieldType>(this SPListItem item, string name, Func<TFieldType, string> getValue, bool throwExeption)
        {
            var retVal = new List<string>();
            var val = item.TryGetFieldValue(name);
            if (val is List<TFieldType>) //  SPFieldUserValueCollection and SPFieldLookUpValueCollection inherit from List<T>
            {
                var collection = (List<TFieldType>)val;
                retVal.AddRange(collection.Select(getValue));
            }
            else if (val is SPFieldMultiChoiceValue)
            {
                // notice: getField in ignored if field value type is SPFieldMultiChoiceValue
                var multiVal = (SPFieldMultiChoiceValue)val;
                var lastValueIndex = multiVal.Count - 1;
                retVal.AddRange(
                    0.To(lastValueIndex)
                     .Select(index => multiVal[index]));

            }
            else if (val is TFieldType)
            {
                retVal.Add(getValue((TFieldType)val));
            }
            else
            {
                if (throwExeption)
                {
                    var exceptionText = "Field {0} was invalid type. Type {1} expected; actual field value type was {2}"
                        .FormatWith(name,
                                    typeof(TFieldType).ToString(),
                                    val.GetType().ToString());
                    throw new InvalidCastException(exceptionText);
                }
                return null;
            }
            return retVal;
        }

        /// <summary>
        /// As GetFieldValueAsStringList TFieldType (this SPListItem item, string name, Func&lt;TFieldType,string&gt; getValue, bool throwExeption) but getValue = (o) => o.ToString() and throwExeption=false;
        /// </summary>
        /// <typeparam name="TFieldType">Field value type: E.g. SPFieldUserValue, object, SPFieldLookUpValue</typeparam>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<string> GetFieldValueAsStringList(this SPListItem item, string name)
        {
            return GetFieldValueAsStringList<object>(item, name, (o) => o.ToString(), false);
        }

        /// <summary>
        /// Gets field. Returns null if resulting value is null. Return list of strings that are supposed to be emails, no validation is include. 
        /// If field is type of number, than the number is converted as string an returned like if it was email address. 
        /// </summary>
        /// <param name="item">SPListItem</param>
        /// <param name="name">The internal or display name of the field to get</param>
        /// <returns></returns>
        public static List<string> TryGetFieldValueAsListOfEmails(this SPListItem item, string name)
        {
            var field = item.TryGetField(name);
            var originalVal = item.TryGetFieldValue(name);
            object val = null;
            if (originalVal == null) return null;
            if (field.Type == SPFieldType.User ||
                field.Type == SPFieldType.Lookup)
            {
                if (originalVal is string)
                {
                    val = field.GetFieldValue(originalVal.ToString());
                }
            }

            val = val ?? originalVal; // if val is not populated do so now

            if (val is SPFieldUserValueCollection ||
               val is SPFieldLookupValueCollection ||
               val is SPFieldMultiChoiceValue)
            {

                if (val is SPFieldUserValueCollection)
                {
                    return GetFieldValueAsStringList<SPFieldUserValue>(
                        item,
                        name,
                        (i) => i.User.Email,
                        false);
                }
                return GetFieldValueAsStringList<object>(
                    item,
                    name,
                    (i) => i.ToString(),
                    false);
            }
            if (val is SPFieldUserValue)
            {
                var userVal = (SPFieldUserValue)val;
                return new List<string>() { userVal.User.Email };
            }
            return new List<string>() { val.ToString() };
        }

        /// <summary>
        /// Gets field value. Returns null if field is not found (or if the field value itself is null!).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to get</param>
        /// <returns></returns>
        public static object TryGetFieldValue(this SPListItem item, string name)
        {
            var field = item.TryGetField(name);
            if (field != null)
                return item[field.StaticName];
            else
                return null;
        }

        /// <summary>
        /// Gets field value. Returns null if field is not found.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to get</param>
        /// <returns></returns>
        public static T? TryGetFieldValueAsNullable<T>(this SPListItem item, string name) where T : struct
        {
            var fieldValue = item.TryGetFieldValue(name);
            if (fieldValue == null) return null;
            if (fieldValue is T)
            {
                return (T)fieldValue;
            }
            if (fieldValue is T?)
            {
                return (T?)fieldValue;
            }
            if (fieldValue is string)
            {
                var str = fieldValue as string;
                if (!string.IsNullOrEmpty(str))
                {
                    if (typeof(T) == typeof(int))
                    {
                        int retVal;
                        object val = (int.TryParse(str, out retVal) ? (object)retVal : null);
                        return (T?)val;
                    }
                    if (typeof(T) == typeof(long))
                    {
                        long retVal;
                        object val = (long.TryParse(str, out retVal) ? (object)retVal : null);
                        return (T?)val;
                    }
                    if (typeof(T) == typeof(Guid))
                    {
                        Guid? id = null;

                        try
                        {
                            id = new Guid(str);
                        }
                        catch
                        {
                            // do nothing; just a bad guid  
                        }

                        return id as T?;
                    }
                    if (typeof(T) == typeof(DateTime))
                    {
                        DateTime retVal;
                        object val = (DateTime.TryParse(str, out retVal) ? (object)retVal : null);
                        return (T?)val;
                    }
                    /// Todo impelement bool and double etc...
                }
            }
            return null;
        }


        /// <summary>
        /// Gets field value. Returns null if field is not found.
        /// </summary>
        /// <typeparam name="T">A nullable reference type, i.e. not int, bool etc.</typeparam>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T TryGetFieldValueAs<T>(this SPListItem item, string name) where T : class
        {
            var fieldValue = item.TryGetFieldValue(name);
            if (fieldValue == null) return null;
            if (fieldValue is T)
            {
                return (T)fieldValue;
            }
            return null;
        }

        /// <summary>
        /// Gets field value casted to the type specified. Returns default(T) if field is not found.
        /// </summary>
        /// <typeparam name="T">Cast value to this type</typeparam>
        /// <param name="item"></param>
        /// <param name="name">The internal or display name of the field to get</param>
        /// <returns></returns>
        public static T TryGetFieldValue<T>(this SPListItem item, string name)
        {
            var field = item.TryGetField(name);

            if (field != null)
                return (T)item[field.StaticName];
            else
                return default(T);
        }

        /// <summary>
        /// Gets the property value from a list item
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static string GetPropertyValue(this SPListItem item, string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                try
                {
                    object o = item.Properties[propertyName];

                    if (o != null)
                    {
                        return o.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the file URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string GetFileUrl(this SPListItem item)
        {
            return SPUrlUtility.CombineUrl(item.Web.Url, item.File.Url);
        }

        /// <summary>
        /// Gets the disp form URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="includeSource">if set to <c>true</c> [include source].</param>
        /// <returns></returns>
        public static string GetDispFormUrl(this SPListItem item, bool includeSource)
        {
            string dispFormUrl = item.ParentList.Forms[(int)PAGETYPE.PAGE_DISPLAYFORM].Url;

            if (includeSource)
            {
                dispFormUrl = string.Format("{0}?ID={1}&Source={2}", dispFormUrl, item.ID, SPHttpUtility.UrlKeyValueEncode(SPContext.Current.Web.Url));
            }
            else
            {
                dispFormUrl = string.Format("{0}?ID={1}", dispFormUrl, item.ID);
            }

            return SPUrlUtility.CombineUrl(item.Web.Url, dispFormUrl);
        }

        /// <summary>
        /// Gets the item date.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static DateTime GetItemDate(this SPListItem item, string fieldName)
        {
            try
            {
                object date = item[fieldName];

                if (date != null) return (DateTime)date;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Gets the item user (CreatedBy / ModifiedBy).
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="getModifiedBy">If true the reference to user who last modified item is returned, otherwise the reference to item author user is returned</param>
        /// <returns></returns>
        public static SPUser GetItemUser(this SPListItem item, bool getModifiedBy)
        {
            try
            {
                object o;
                if (getModifiedBy)
                {
                    o = item[SPBuiltInFieldId.Editor];
                }
                else
                {
                    o = item[SPBuiltInFieldId.Author];
                }
                string[] tmp = ((String)o).Split(new char[] { ';' });
                int uid;
                if (Int32.TryParse(tmp[0], out uid))
                {
                    return item.ParentList.ParentWeb.Users.GetByID(uid);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets the item user from specified field 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">field name where user is retrieved from. If field contains several users, returns only first user.</param>
        /// <returns></returns>
        public static SPUser GetItemUserFromCustomfield(this SPListItem item, string fieldName)
        {
            try
            {
                object o;
                if (item.ParentList.Fields.ContainsField(fieldName))
                {
                    SPField field = item.ParentList.Fields.GetField(fieldName);
                    if (field.Type == SPFieldType.User)
                    {
                        o = item[fieldName];
                        string[] tmp = ((String)o).Split(new char[] { ';' });
                        int uid;
                        if (Int32.TryParse(tmp[0], out uid))
                        {
                            return item.ParentList.ParentWeb.Users.GetByID(uid);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ex.LogError();
            }
            return null;
        }
    }
}
