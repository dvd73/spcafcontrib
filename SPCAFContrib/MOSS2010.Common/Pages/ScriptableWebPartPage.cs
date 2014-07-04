using Microsoft.SharePoint.WebPartPages;
using MOSS.Common.Code;

namespace MOSS.Common.Pages
{
    public class ScriptableWebPartPage : WebPartPage, ICustomScript
    {
        public CustomScriptType ScriptType { get; set; }
    }
}
