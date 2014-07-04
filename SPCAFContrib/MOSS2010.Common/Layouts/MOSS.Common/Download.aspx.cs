using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace MOSS.Common.ApplicationPage
{
    public partial class Download : UnsecuredLayoutsPageBase
    {
        protected override bool AllowAnonymousAccess
        {
            get { return true; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string idStr = Request.Params["ID"];

            if (!String.IsNullOrEmpty(idStr))
            {
                Guid  id = new Guid(idStr);
                //SPSecurity.RunWithElevatedPrivileges(() =>
                //                                         {
                //                                             using (var site = new SPSite(SPContext.Current.Site.ID))
                //                                             {
                //                                                 using (var web = site.OpenWeb(SPContext.Current.Web.ID)
                //                                                     )
                //                                                 {
                //                                                     var item = web.Lists[listID].Items.GetItemById(id);

                //                                                     var fileName =
                //                                                         item[SPBuiltInFieldId.Title].ToString();
                //                                                     Response.ClearContent();
                //                                                     Response.ClearHeaders();
                //                                                     Response.AppendHeader("Content-Disposition",
                //                                                                           "attachment; filename= " +
                //                                                                           "\"" + fileName + "\"");
                //                                                     Response.ContentType =
                //                                                         Response.ContentType =
                //                                                         "application/octet-stream";

                //                                                     SPFile tempFile = web.GetFile(item.File.Url);

                //                                                     var obj = tempFile.OpenBinary();
                //                                                     if (Response.IsClientConnected)
                //                                                     {
                //                                                         Response.BinaryWrite(obj);
                //                                                         Response.Flush();
                //                                                         Response.End();
                //                                                     }
                //                                                     else
                //                                                         Response.Close();
                //                                                 }
                //                                             }
                //                                         });
            }
        }
    }
}
