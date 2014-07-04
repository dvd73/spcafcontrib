using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using MOSS.Common.Code;
using MOSS.Common.Pages;

namespace MOSS.Common.Controls
{
    public partial class IssueInformerUserControl : UserControl
    {
        public Color ForeColor
        {
            get {return lblMessage.ForeColor;}
            set {lblMessage.ForeColor = value;}
        }

        public bool FontBold
        {
            get {return lblMessage.Font.Bold;}
            set {lblMessage.Font.Bold = value;}
        }

        public FontUnit FontSize
        {
            get { return lblMessage.Font.Size; }
            set { lblMessage.Font.Size = value; }
        }

        public string IssueMessage
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; lblMessage.Visible = true; }
        }

        public string IssueDetails
        {
            get { return ltrDetails.Text; }
            set { ltrDetails.Text = value; pnlDetailsDialog.Visible = true; imgMore.Visible = true; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ScriptRequestEventArgs ScriptRequest = new ScriptRequestEventArgs();
            ScriptRequest.ScriptType = CustomScriptType.JQueryScript | CustomScriptType.JQueryUIScript | CustomScriptType.JQueryUITheme;

            RaiseBubbleEvent(this, ScriptRequest);

            lblMessage.Visible = false;
            pnlDetailsDialog.Visible = false;
            imgMore.Visible = false;
        }
        
    }
}
