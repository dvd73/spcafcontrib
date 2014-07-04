using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;

namespace MOSS.Common.Utilities
{
    public class PeoplePickerHelper
    {
        public static string GetPeoplePickerLogin(ControlCollection controlCollection)
        {
            var result = string.Empty;
            foreach (Control control in controlCollection)
            {
                var peopleEditor = control as PeopleEditor;
                if (peopleEditor != null)
                {
                    result = peopleEditor.CommaSeparatedAccounts;
                    return result;
                }
                if (control.HasControls())
                {
                    result = GetPeoplePickerLogin(control.Controls);
                }
            }
            return result;
        }

        public static SPPrincipalInfo GetPeoplePickerUser(ControlCollection controlCollection)
        {
            SPPrincipalInfo result = null;
            foreach (Control control in controlCollection)
            {
                var peopleEditor = control as PeopleEditor;
                if (peopleEditor != null && peopleEditor.ResolvedEntities.Count == 1)
                {
                    PickerEntity pickerEntity = (PickerEntity)peopleEditor.ResolvedEntities[0];
                    result = MOSSUserHelper.Instance.GetPrincipalInfo(SPContext.Current.Web.Site.WebApplication, pickerEntity.Key);
                    return result;
                }
                if (control.HasControls())
                {
                    result = GetPeoplePickerUser(control.Controls);
                }
            }
            return result;
        }
    }
}
