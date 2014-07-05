using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;
using SPCAFContrib.Rules.Code.Base;

namespace SPCAFContrib.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
     CheckId = CheckIDs.Rules.Assembly.AvoidUsingUpdatePanelInCode,
     Help = CheckIDs.Rules.General.AvoidUsingUpdatePanel_HelpUrl,

     Message = "Avoid using of UpdatePanel in code.",
     DisplayName = "Avoid using of UpdatePanel in code.",
     Description = "The ASP.NET AJAX UpdatePanel control must change the page’s postback behavior to enable support for asynchronous postbacks and partial rendering. However, in a SharePoint application, these modifications cause a JavaScript error. This is because SharePoint attempts to make a similar change.",
     Resolution = "When possible try to avoid using of UpdatePanel, preferring jQuery.ajax() instead.",

     DefaultSeverity = Severity.Warning,
     SharePointVersion = new[] {"12", "14"},

     Links = new[]
     {
         "Integrating ASP.NET AJAX with SharePoint",
         "http://www.slideshare.net/rob.windsor/integrating-aspnet-ajax-with-sharepoint-presentation",
         "WSS 3.0: Creating a Basic ASP.NET AJAX-enabled Web Part",
         "http://msdn.microsoft.com/en-us/library/bb861877.aspx",
         "Using the AJAX Update Panel in SharePoint",
         "http://msdn.microsoft.com/en-us/library/ff650218.aspx",
         "UpdatePanel Class",
         "http://msdn.microsoft.com/en-us/library/system.web.ui.updatepanel.aspx"
     }
     )]
    public class AvoidUsingUpdatePanelInCode : SearchMethodRuleBase
    {
        protected override void PopulateTypeMap()
        {
            TargetTypeMap.Add(TypeKeys.UpdatePanel, new List<string>{"ctor"});
        }
    }
}
