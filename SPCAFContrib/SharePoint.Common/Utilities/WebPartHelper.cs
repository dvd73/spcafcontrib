
namespace SharePoint.Common.Utilities
{
    public class WebPartHelper : SingletoneHelper
    {
        #region Singeton interface

        public static WebPartHelper Instance
        {
            get { return GetInstance<WebPartHelper>(); }
        }

        #endregion        
    }
}
