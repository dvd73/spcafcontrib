using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace SPCAFContrib.Test.UnsafeStringComporation
{
    public class UnsafeStringComporationImpl
    {
        private const string TRACECATEGORY = "SharePoint.Core.SharePoint.UnsafeStringComporationImpl";

        #region negative

        public void Negative_Linq_Expression()
        {
            SPList list = null;
            var pageLayoutName = "test";

            var pageLayout = list.Lists.OfType<SPListItem>()
                                        .FirstOrDefault(item => item.Name == pageLayoutName);
        }

        public void Negative_SPContentType_Name()
        {
            SPFolder folder = null;

            foreach (SPFile file in folder.Files)
            {
                SPListItem cItem = file.Item;
                if (cItem.ContentType.Name == "test" && cItem.DoesUserHavePermissions(SPBasePermissions.ViewListItems))
                {
                    Negative_SPContentType_Name();
                }
            }
        }

        #endregion
    }
}
