using System;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using MOSS.Common.Code;

namespace MOSS.Common.ApplicationPage
{
    public partial class Upload : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Files.Count == 1)
            {
                // 1. закачиваем файл в БД
                // 2. отображаем ВСЕ загруженные файлы в репитере
                rptFiles.DataSource = new UploadedFile[] { new UploadedFile() { Id = new Guid("{E28D014C-0977-4B42-A7E8-4918DB6C76A9}"), Name = Request.Files[0].FileName, Url = "http://www.digdes.ru", Description = "тестовый файл" } };
                rptFiles.DataBind();
            }
        }

        protected void btnDeleteFile_Click(object sender, EventArgs e)
        {

        }       

        protected void rptFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
            {
                HyperLink lnkFile = e.Item.FindControl("lnkFile") as HyperLink;
                lnkFile.NavigateUrl = String.Format(lnkFile.NavigateUrl, ((UploadedFile)e.Item.DataItem).Id);
            }            
        }
    }
}
