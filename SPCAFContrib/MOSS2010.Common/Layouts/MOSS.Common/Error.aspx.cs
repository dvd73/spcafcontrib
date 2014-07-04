using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using MOSS.Common.Controls;
using SharePoint.Common.Utilities;
using MOSS.Common.Utilities;

namespace MOSS.Common.Layouts
{
    public partial class Error : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                IssueInformerUserControl ctlIssueInformer = ctlErrorMessage as IssueInformerUserControl;
                ctlIssueInformer.IssueMessage = "Подробности";
                string last_error = String.IsNullOrEmpty(MOSSLogger.Instance.LastException) ? Logger.Instance.LastException : MOSSLogger.Instance.LastException;
                SPUser user = UserHelper.Instance.CurrentUser;
                if (user != null)
                    last_error += "<br/> <strong>Current user:</strong> " + user.Name + "(" + UserHelper.Instance.CurrentUserLoginName + ")";
                last_error += "<br/> <strong>Request URL: </strong> " + this.CurrentRequestUrlAndQuery;
                last_error += "<br/> <strong>Date: </strong> " + DateTime.Now;
                if (!String.IsNullOrEmpty(Logger.Instance.LastException))
                    last_error += "<br/> <i>Logged to Application Log list</i>" ;
                else
                    last_error += "<br/> <i>Logged to Event Viewer application log</i>";
                ctlIssueInformer.IssueDetails = last_error.Replace(Environment.NewLine, "<br/>");
                ctlIssueInformer.Visible = true;
            }
        }
    }
}
