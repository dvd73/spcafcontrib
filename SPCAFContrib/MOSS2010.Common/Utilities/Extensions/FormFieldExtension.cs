using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace MOSS.Common.Utilities.Extensions
{
    public static class FormFieldExtension
    {
        public static string GetUserLogin(this BaseFieldControl formField)
        {
            return PeoplePickerHelper.GetPeoplePickerLogin(formField.Controls);
        }

        public static SPPrincipalInfo GetUser(this BaseFieldControl formField)
        {
            return PeoplePickerHelper.GetPeoplePickerUser(formField.Controls);
        }

        
    }
}
