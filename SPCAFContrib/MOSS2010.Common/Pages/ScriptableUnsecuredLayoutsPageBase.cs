using Microsoft.SharePoint.WebControls;
using MOSS.Common.Code;

namespace MOSS.Common.Pages
{
    public class ScriptableUnsecuredLayoutsPageBase : UnsecuredLayoutsPageBase, ICustomScript
    {
        public CustomScriptType ScriptType { get; set; }
    }
}
