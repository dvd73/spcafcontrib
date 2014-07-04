using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using SharePoint.Common.Utilities;
using SharePoint.Common.Utilities.Extensions;

namespace MOSS.Common.Utilities
{
    public class MOSSSPListHelper : ListHelper
    {
        public new static MOSSSPListHelper Instance
        {
            get { return GetInstance<MOSSSPListHelper>(); }
        }

        #region ContentTypes

        public void ModifyNewItemMenu(SPList list, string contentTypeName, AddRemoveModificationType action)
        {
            IList<SPContentType> currentOrder = list.RootFolder.ContentTypeOrder;
            List<SPContentType> result = new List<SPContentType>();

            try
            {
                if (Regex.IsMatch(contentTypeName, "^0x", RegexOptions.IgnoreCase))
                {
                    var contentType = list.ContentTypes[list.ContentTypes.BestMatch(new SPContentTypeId(contentTypeName))];
                    contentTypeName = contentType.Name;
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            switch (action)
            {
                case (AddRemoveModificationType.Add):
                    var newOrder = new List<SPContentType>(currentOrder);
                    foreach (SPContentType ct in list.ContentTypes)
                    {
                        if (ct.Name.Equals(contentTypeName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!newOrder.Select(c => c.Name).Contains(ct.Name))
                                newOrder.Add(ct);
                        }
                    }
                    list.RootFolder.Properties["vti_contenttypeorder"] = newOrder.Select(ct => ct.Id).ConcatenateIntoString(",").Trim(',');
                    list.RootFolder.Update();
                    break;

                case (AddRemoveModificationType.Remove):
                    foreach (SPContentType ct in currentOrder)
                    {
                        if (!ct.Name.Equals(contentTypeName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            result.Add(ct);
                        }
                    }
                    list.RootFolder.UniqueContentTypeOrder = result;
                    list.RootFolder.Update();
                    break;
            }
        }
        #endregion
    }

    public enum AddRemoveModificationType
    {
        Add,
        Remove
    }
}
