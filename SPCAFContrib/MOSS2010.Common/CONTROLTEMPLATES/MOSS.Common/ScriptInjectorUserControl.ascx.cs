using System;
using System.Web.UI;
using MOSS.Common.Pages;
using Microsoft.SharePoint.WebControls;
using MOSS.Common.Code;
using SharePoint.Common.Utilities;

namespace MOSS.Common.Controls
{
    public partial class ScriptInjectorUserControl : UserControl
    {
        #region Methods

        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.Page.Master is BaseMasterPage)
            {
                (this.Page.Master as BaseMasterPage).ScriptInjector = this;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.Page is ICustomScript)
            {
                CheckScripts(this.Page as ICustomScript);
            }
        }

        private void CheckScripts(ICustomScript scriptRequest)
        {
            if ((scriptRequest.ScriptType & CustomScriptType.JQueryScript) == CustomScriptType.JQueryScript)
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery.min.js", false);

            if ((scriptRequest.ScriptType & CustomScriptType.JQueryUIScript) == CustomScriptType.JQueryUIScript)
            {
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery-ui.custom.min.js", false);
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery.ui.datepicker-ru.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.JQueryUITheme) == CustomScriptType.JQueryUITheme)
            {
                CssRegistration cs = new CssRegistration();
                cs.After = "corev4.css";
                cs.Name = UrlHelper.SharePointUrlToRelativeUrl("~site/_layouts/MOSS.Common/styles/smoothness/jquery-ui.custom.min.css");
                this.Controls.Add(cs);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.TimePickerScript) == CustomScriptType.TimePickerScript)
            {
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery.ui.timepicker.js", false);
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery.ui.timepicker-ru.js", false);
                CssRegistration cs = new CssRegistration();
                cs.After = "corev4.css";
                cs.Name = UrlHelper.SharePointUrlToRelativeUrl("~site/_layouts/MOSS.Common/styles/jquery.ui.timepicker.css");
                this.Controls.Add(cs);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.ExpandScript) == CustomScriptType.ExpandScript)
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/expand.js", false);

            if ((scriptRequest.ScriptType & CustomScriptType.SPServiceScript) == CustomScriptType.SPServiceScript)
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery.SPServices.min.js", false);

            if ((scriptRequest.ScriptType & CustomScriptType.ListAttachments) == CustomScriptType.ListAttachments)
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/ListAttachments.js", false);

            if ((scriptRequest.ScriptType & CustomScriptType.ResizeIframe) == CustomScriptType.ResizeIframe)
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/resize_iframe.js", false);

            if ((scriptRequest.ScriptType & CustomScriptType.HideRibbonInDlg) == CustomScriptType.HideRibbonInDlg)
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/hide.ribbon.dlg.js", false);

            if ((scriptRequest.ScriptType & CustomScriptType.ActivateRibbonReadTab) == CustomScriptType.ActivateRibbonReadTab)
            {
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/jquery.query.js", false);
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Scripts/set_ribbon_read_tab.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.FlowPlayer) ==CustomScriptType.FlowPlayer)
            {
                ScriptLink.Register(this.Page, "/_layouts/MOSS.Common/Flowplayer/flowplayer.min.js", false);
                CssRegistration cs = new CssRegistration();
                cs.After = "corev4.css";
                cs.Name = UrlHelper.SharePointUrlToRelativeUrl("~site/_layouts/MOSS.Common/Flowplayer/skin/playful.css");
                this.Controls.Add(cs);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.SuperFishMenu) == CustomScriptType.SuperFishMenu)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/hoverIntent.js", false);
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/superfish.js", false);
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/supersubs.js", false);
                CssRegistration cs = new CssRegistration();
                cs.After = "corev4.css";
                cs.Name = UrlHelper.SharePointUrlToRelativeUrl("~site/_layouts/MOSS.Common/styles/superfish.css");
                this.Controls.Add(cs);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.JQueryCookies) == CustomScriptType.JQueryCookies)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/jquery.cookie.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.JQueryHotkeys) == CustomScriptType.JQueryHotkeys)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/jquery.hotkeys.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.JQueryQuickLaunch) == CustomScriptType.JQueryQuickLaunch)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/jQuery.LISP.quicklaunch.min.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.JSTree) == CustomScriptType.JSTree)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/jsTree/jquery.jstree.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.DynaTree) == CustomScriptType.DynaTree)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/dynatree/jquery.dynatree.min.js", false);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.Bootstrap) == CustomScriptType.Bootstrap)
            {                
                ScriptLink.Register(this.Page, "/MOSS.Common/bootstrap/js/bootstrap.min.js", false);
                CssRegistration cs = new CssRegistration();
                cs.After = "corev4.css";
                cs.Name = UrlHelper.SharePointUrlToRelativeUrl("~site/_layouts/MOSS.Common/bootstrap/css/bootstrap.min.css");
                this.Controls.Add(cs);
            }

            if ((scriptRequest.ScriptType & CustomScriptType.JQueryValidation) == CustomScriptType.JQueryValidation)
            {
                ScriptLink.Register(this.Page, "/MOSS.Common/Scripts/jquery.validate.min.js", false);
            }
        }

        public void SetScripts(ICustomScript scriptRequest)
        {
            CheckScripts(scriptRequest);
        }

        #endregion

    }
}
