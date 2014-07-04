using Microsoft.SharePoint.WebControls;
using MOSS.Common.Code;

namespace MOSS.Common.Pages
{
    public class ScriptableLayoutsPageBase : LayoutsPageBase, ICustomScript
    {
        public CustomScriptType ScriptType { get; set; }
    }
}
