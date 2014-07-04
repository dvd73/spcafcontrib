/* This is a utility class that will allow you to force an update/refresh of a
 * list using BDC data.
 *
 * Sample usage:
 *
 *         
            using (SPSite site = new SPSite(rootURL))
            {
               using (SPWeb web = site.OpenWeb())
               {
                  SPList list = web.Lists[listName];
 
                  BusinessDataColumnUpdater refresher =
                     new BusinessDataColumnUpdater(list, columnName, sspName);
 
                  //if you want to have a verbose message trail of what happened
                  //when you ran the code, turn the EnableTracing flag to true
                  refresher.EnableTracing = true;
 
                  //call the update method
                  refresher.UpdateColumnUsingBatch();
 
                  //write out the Trace Messages if you chose to have those
                  Console.WriteLine(refresher.TraceMessages.ToString());
               }
            }
 *
 *
 *
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Office.Server.ApplicationRegistry.Infrastructure;
using Microsoft.SharePoint.Portal.WebControls;
using Microsoft.Office.Server.ApplicationRegistry.MetadataModel;
using Microsoft.SharePoint.BusinessData.SharedService;
using Microsoft.Office.Server.ApplicationRegistry.Runtime;

namespace MOSS.Common.Utilities
{
    public class BCDHelper
    {

        /// <summary>
        /// Set all secondary BCS fields for a given entity
        /// </summary>
        /// <param name="listItem">The item to set the fields for</param>
        /// <param name="dataField">The BCS field itself</param>
        /// <param name="entityInstance">The entity to get the values from</param>
        private static void SetSecondaryFields(SPListItem listItem, SPBusinessDataField dataField, IEntityInstance entityInstance)
        {
            // Convert the entity to a formatted datatable
            DataTable dtBDCData = entityInstance.EntityAsFormattedDataTable;

            // Set the BCS field itself (Display Value)
            listItem[dataField.Id] = dtBDCData.Rows[0][dataField.BdcFieldName].ToString();

            // Get the specific finder method to get the columns that returns
            MethodInstance method = entityInstance.Entity.GetSpecificFinderMethodInstance();
            TypeDescriptorCollection oDescriptors = method.GetReturnTypeDescriptor().GetChildTypeDescriptors()[0].GetChildTypeDescriptors();

            // Set the column names to the correct values
            foreach (Microsoft.Office.Server.ApplicationRegistry.MetadataModel.TypeDescriptor oType in oDescriptors)
            {
                if (oType.ContainsLocalizedDisplayName())
                {
                    if (dtBDCData.Columns.Contains(oType.Name))
                    {
                        dtBDCData.Columns[oType.Name].ColumnName = oType.GetLocalizedDisplayName();
                    }
                }
            }

            // get the secondary field display names; these should be set
            string[] sSecondaryFieldsDisplayNames = dataField.GetSecondaryFieldsNames();

            // loop through the fields and set each column to its value
            foreach (string columnNameint in sSecondaryFieldsDisplayNames)
            {
                Guid gFieldID = listItem.Fields[String.Format("{0}: {1}", dataField.Title, columnNameint)].Id;
                listItem[gFieldID] = dtBDCData.Rows[0][columnNameint].ToString();
            }
        }
    }

    public class BusinessDataColumnUpdater
    {
        #region private Data members

        protected SPList _list = null;
        protected string _columnName = "";
        protected string _sharedResourceProvider = "";
        protected Microsoft.Office.Server.ApplicationRegistry.MetadataModel.LobSystemInstance _lobSysInst = null;
        protected Microsoft.Office.Server.ApplicationRegistry.MetadataModel.Entity _entity = null;
        protected Microsoft.Office.Server.ApplicationRegistry.MetadataModel.View _specificFinderView = null;
        protected SPListItemCollection _items = null;
        protected StringBuilder _traceMessages = new StringBuilder();
        protected bool _enableTracing = false;
        protected string _batchFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
           "<ows:Batch OnError=\"Return\">{0}</ows:Batch>";
        protected string _batch = string.Empty;
        protected StringBuilder _methodBuilder = new StringBuilder();

        #endregion

        #region Constructors

        public BusinessDataColumnUpdater() { }

        public BusinessDataColumnUpdater(SPList list, string businessDataColumnName, string sharedResourceProvider)
        {
            _list = list;
            _columnName = businessDataColumnName;
            _sharedResourceProvider = sharedResourceProvider;
        }

        public BusinessDataColumnUpdater(SPList list, string businessDataColumnName, string sharedResourceProvider, bool enableTracing)
        {
            _list = list;
            _columnName = businessDataColumnName;
            _sharedResourceProvider = sharedResourceProvider;
            _enableTracing = enableTracing;
        }

        #endregion

        #region Properties

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        public SPList List
        {
            get { return _list; }
            set { _list = value; }
        }

        public string SharedResourceProvider
        {
            get { return _sharedResourceProvider; }
            set { _sharedResourceProvider = value; }
        }


        public bool EnableTracing
        {
            get { return _enableTracing; }
            set { _enableTracing = value; }
        }

        public StringBuilder TraceMessages
        {
            get { return _traceMessages; }
            private set { _traceMessages = value; }
        }

        #endregion

        #region Public Methods

        /* Note: This is the public method that you should call in a production
       * environment. 
       * */
        public virtual void UpdateColumnUsingBatch()
        {
            if (_enableTracing && _traceMessages.Length > 0)
                _traceMessages = new StringBuilder();

            //check to make sure the field is a valid column name
            if (!(_list.Fields.ContainsField(_columnName)))
            {
                throw new
                   ArgumentException("The field '" + _columnName +
                   "' is not a valid field name in this list - check for typos!");
            }

            SPField fieldByInternalName =
               _list.Fields.GetFieldByInternalName(_columnName);

            //check to make sure that the field is a business data column
            if (!(fieldByInternalName is BusinessDataField))
            {
                throw new
                   BusinessDataListConfigurationException(
                     "The field " + _columnName + " is not a business data field");
            }

            //get the bdc data column in the list
            BusinessDataField bizDataField = (BusinessDataField)fieldByInternalName;
            string relatedFieldName = bizDataField.RelatedField;

            //build a list of related fields in the list that derive their data from
            //the bdc data column
            string[] secondaryFieldsNames = bizDataField.GetSecondaryFieldsNames();
            string[] secondaryWssFieldNames = new string[0];
            string property = bizDataField.GetProperty("SecondaryFieldWssNames");

            //populate the array of secondary names
            secondaryWssFieldNames = property.Split(new char[] { ':' });

            SqlSessionProvider.Instance().SetSharedResourceProviderToUse(
               _sharedResourceProvider);

            _lobSysInst =
               Microsoft.Office.Server.ApplicationRegistry.MetadataModel.ApplicationRegistry.GetLobSystemInstanceByName(
               bizDataField.SystemInstanceName);
            _entity = _lobSysInst.GetEntities()[bizDataField.EntityName];
            _specificFinderView = _entity.GetSpecificFinderView();

            int totalListItems = _list.ItemCount;

            if (_enableTracing)
            {
                _traceMessages.AppendLine("Connection Info for Clean Up");
                _traceMessages.AppendLine("-------------------------------");
                _traceMessages.AppendLine("LOB SystemInstanceName: " + _lobSysInst.Name);
                _traceMessages.AppendLine("LOB System Entity Name: " + _entity.Name);
                _traceMessages.AppendLine("List: " + _list.Title);
                _traceMessages.AppendLine("List Items Found: " + totalListItems);
                _traceMessages.AppendLine("-------------------------------");
                _traceMessages.AppendLine("");
                _traceMessages.AppendLine("Attempting to update list items....");
                _traceMessages.AppendLine("");
            }

            //get the list item collection from the list limited by the total number
            //of items in the list and return only the main field and its related
            //BDC field that we want to update.  this should be an efficient way to
            //work with a large list in a production environment.
            SPQuery query = new SPQuery();
            query.RowLimit = totalListItems < 2000 ? (uint)totalListItems : (uint)2000;
            query.ViewFields = string.Format("<FieldRef Name='{0}'/><FieldRef Name='{1}'/>",
               _columnName, relatedFieldName);
            query.ViewAttributes = "Scope=\"Recursive\""; //you must set this value to recursive to update things in subfolders

            //retrieve SPListItemCollection using paging techniques for optimization
            do
            {
                _items = _list.GetItems(query);
                _methodBuilder = new StringBuilder();
                //loop through list items in list and try to update/refresh with latest BDC value
                foreach (SPListItem item in _items)
                {
                    //make sure the item has data in the column before trying to update
                    if (!(item[_columnName] == null) && !(item[relatedFieldName] == null))
                    {
                        BuildListItemBatchUpdateCAML(item, bizDataField, _lobSysInst, _entity,
                           _specificFinderView, secondaryFieldsNames, secondaryWssFieldNames);
                    }
                } //end foreach

                // Put the pieces together.
                _batch = string.Format(_batchFormat, _methodBuilder.ToString());

                // Process the batch of commands.
                string batchReturn = _list.ParentWeb.ProcessBatchData(_batch);

                if (_enableTracing)
                {
                    _traceMessages.AppendLine("-------------------------------");
                    _traceMessages.AppendLine("OUTCOME for Batch");
                    _traceMessages.AppendLine("-------------------------------");
                    _traceMessages.AppendLine(batchReturn);
                }

                // set the position cursor for the next iteration
                query.ListItemCollectionPosition = _items.ListItemCollectionPosition;
            } while (query.ListItemCollectionPosition != null);

        } //end method

        #endregion

        #region Private methods

        /* Note: this is a more efficient method used by the UpdateColumnUsingBatch()
       * method that builds a dynamic batch CAML fragment for a batch update.
       *
       * */
        protected virtual void BuildListItemBatchUpdateCAML(SPListItem item, BusinessDataField bizDataField,
           LobSystemInstance _lobSysInst, Entity _entity,
           Microsoft.Office.Server.ApplicationRegistry.MetadataModel.View view,
           string[] secondaryBdcFieldNames, string[] secondaryWssFieldNames)
        {
            string bdcFieldName = bizDataField.BdcFieldName;
            string encodedId = null;
            object[] objArray;
            IList<object> identifierValues = null;
            object[] objArray2 = null;
            List<Field>.Enumerator enumerator;

            encodedId = (string)item[bizDataField.RelatedField];
            StringBuilder methodFormat = new StringBuilder();

            try
            {
                if (encodedId != null)
                {
                    objArray = EntityInstanceIdEncoder.DecodeEntityInstanceId(encodedId);
                    identifierValues =
                       _entity.FindSpecific(objArray, _lobSysInst).GetIdentifierValues();
                    objArray2 = new object[identifierValues.Count];
                }

                for (int i = 0; i < identifierValues.Count; i++)
                {
                    objArray2[i] = identifierValues[i];
                }

                item[bizDataField.RelatedField] =
                   EntityInstanceIdEncoder.EncodeEntityInstanceId(objArray2);
                enumerator = view.Fields.GetEnumerator();

                Microsoft.Office.Server.ApplicationRegistry.Runtime.IEntityInstance instance =
                   _entity.FindSpecific(objArray2, _lobSysInst);

                Field field;
                string name;

                //loop through all of the fields for the current list item.  when the bdc
                //business data column is found, obtain the most recent data from the data source
                //and update the list item with that value.  similarly, loop through any related
                //fields and refresh them as well
                while (enumerator.MoveNext())
                {
                    field = enumerator.Current;
                    name = field.Name;
                    if (name == bdcFieldName)
                    {
                        //this sets the current value for the field from the database via the bdc
                        item[bizDataField.InternalName] =
                           Convert.ToString(instance.GetFormatted(field));

                        methodFormat.AppendFormat(
                           "<Method ID=\"{0}\">" +
                           "<SetList>{1}</SetList>" +
                           "<SetVar Name=\"Cmd\">Save</SetVar>" +
                           "<SetVar Name=\"ID\">{2}</SetVar>" +
                           "<SetVar Name=\"urn:schemas-microsoft-com:office:office#{3}\">{4}</SetVar>",
                           item.ID, _list.ID.ToString(), item.ID,
                           bizDataField.InternalName,
                           Convert.ToString(instance.GetFormatted(field)));
                    }

                    if (secondaryBdcFieldNames.Length > 0)
                    {
                        //loop through the secondary fields to refresh those as well
                        for (int i = 0; i < secondaryBdcFieldNames.Length; i++)
                        {
                            //if a secondary field is found, update it
                            if (secondaryBdcFieldNames[i] == field.Name)
                            {
                                item[secondaryWssFieldNames[i]] =
                                   Convert.ToString(instance.GetFormatted(field));

                                methodFormat.AppendFormat(
                                   "<SetVar Name=\"urn:schemas-microsoft-com:office:office#{0}\">{1}</SetVar>",
                                   secondaryWssFieldNames[i],
                                   Convert.ToString(instance.GetFormatted(field))
                                   );

                            } //end if
                        } //end for

                    }
                } //end while

                methodFormat.Append("</Method>");
                _methodBuilder.Append(methodFormat.ToString());

            } //end try
            catch (Microsoft.Office.Server.ApplicationRegistry.Runtime.ObjectNotFoundException onfex)
            {
            }
            catch (Exception genericErr)
            {
            }

        } //end method

        #endregion

    } //end class

}
