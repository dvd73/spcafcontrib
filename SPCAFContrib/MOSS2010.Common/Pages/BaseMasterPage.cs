using System;
using MOSS.Common.Code;
using MOSS.Common.Controls;
using MOSS.Common.Utilities;
using System.Web.Configuration;
using Microsoft.SharePoint;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;
using SharePoint.Common.Utilities;
using SharePoint.Common.Utilities.Extensions;

namespace MOSS.Common.Pages
{
	public class BaseMasterPage : System.Web.UI.MasterPage, ICustomErrorPage
	{
		public ScriptInjectorUserControl ScriptInjector { get; set; }

	    public virtual string ErrorPageUrl
	    {
	        get
	        {
	            return "/_layouts/MOSS.Common/Error.aspx";
	        }
	    }

		public BaseMasterPage()
		{
            SPSite site = SPContext.Current.Site;			
            try
            {
                System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                CustomErrorsSection section = (CustomErrorsSection)configuration.GetSection("system.web/customErrors");
                UseCustomeErrorPage = section.Mode != CustomErrorsMode.Off;    
            }
            catch (Exception ex)
            {
                ex.LogError(String.Format("Unable to set UseCustomeErrorPage variable for site {0}", site.Url));
            }
			
		}

		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			if (e is ScriptRequestEventArgs)
			{
				ScriptRequestEventArgs ScriptRequest = e as ScriptRequestEventArgs;

				if (ScriptInjector != null)
				{
					ScriptInjector.SetScripts(ScriptRequest);

					return true;
				}                
			}

			return false;
		}

		protected void Page_Error(object sender, EventArgs e)
		{
			MOSSLogger.Instance.LogError(this.Context.Error, this.Request.Url);

			// to prevent the error from continuing to the Application_Error event handler (which would subsequently invoke the SharePoint error handling infrastucture)
			this.Context.ClearError();

			// displaying a friendly error message to the user, while still preserving the URL of the original request
            this.Server.Transfer(ErrorPageUrl);
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			// If an unhandled exception occurs, then SharePoint transfers the
			// request to /_layouts/error.aspx (via SPRequestModule). This
			// causes a couple of issues:
			//
			// 1) The SharePoint error.aspx page is hard-coded to use
			// simple.master and therefore looks like an OOTB SharePoint page
			// (i.e. the page is not branded like the rest of the site)
			//
			// 2) The SharePoint error.aspx page displays the following
			// message: "Troubleshoot issues with Windows SharePoint Services"
			// (which we obviously don't want to display on an Internet-facing
			// site)
			//
			// To avoid these issues, add a custom error handler for the page

			if (UseCustomeErrorPage)
				this.Page.Error += new EventHandler(Page_Error);
			
			//ScriptRequestEventArgs ScriptRequest = new ScriptRequestEventArgs();
			//ScriptRequest.ListAttachments = true;
			//ScriptRequest.JQueryScript = true;
			//if (ScriptInjector != null) ScriptInjector.SetScripts(ScriptRequest);

			CssRegistration cs = new CssRegistration();
			cs.After = "corev4.css";
			cs.Name = UrlHelper.SharePointUrlToRelativeUrl("~sitecollection/_layouts/MOSS.Common/styles/common.css");
			this.Controls.Add(cs);            
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			string script = @"var argObj = {
				hideIdColumn:true,
				clickToOpen:false,
				clickMouseover:'Click to open',
				oneClickOpenIfSingle:true};
			  
				customListAttachments(argObj);";

			//ScriptManager.RegisterStartupScript(this, this.GetType(), "{6751F750-305B-4681-8FFD-F929D51B3DBE}", script, true);
		}

		public bool UseCustomeErrorPage { get; set; }
	}
}
