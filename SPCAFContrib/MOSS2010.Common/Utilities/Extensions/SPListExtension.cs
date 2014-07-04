using Microsoft.SharePoint;
using MOSS.Common.Utilities;

namespace MOSS.Common.Utilities.Extensions
{
    public static class SPListExtension
    {
        public static void HideNewMenuItem(this SPList list, string contentTypeName)
        {
            MOSSSPListHelper.Instance.ModifyNewItemMenu(list, contentTypeName, AddRemoveModificationType.Remove);
        }

        public static void ShowNewMenuItem(this SPList list, string contentTypeName)
        {
            MOSSSPListHelper.Instance.ModifyNewItemMenu(list, contentTypeName, AddRemoveModificationType.Add);
        }
    }
}
