using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePoint.Common.Utilities;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using System.Collections.Generic;


namespace MOSS.Common.Utilities
{
    public class MOSSPermissionHelper : PermissionHelper 
    {
        #region Singeton interface

        public new static MOSSPermissionHelper Instance
        {
            get { return GetInstance<MOSSPermissionHelper>(); }
        }

        #endregion 

        public bool IsSiteCollAdmin(HttpContext ctx)
        {
            SPWeb web = SPControl.GetContextWeb(ctx);
            return IsAuthenticated(ctx) && IsSiteCollAdmin(web);
        }
    }
}
