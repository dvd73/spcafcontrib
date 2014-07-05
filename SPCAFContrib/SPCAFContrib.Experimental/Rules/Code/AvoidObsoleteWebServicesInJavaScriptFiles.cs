using System;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.Groups;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribBestPracticesGroup),
    CheckId = CheckIDs.Rules.Assembly.AvoidObsoleteWebServicesInJavaScriptFiles,
    DisplayName = "AvoidObsoleteWebServicesInJavaScriptFiles",
    Description = "AvoidObsoleteWebServicesInJavaScriptFiles",
    DefaultSeverity = Severity.Information,
    SharePointVersion = new[] { "15" },
    Message = "AvoidObsoleteWebServicesInJavaScriptFiles",
    Resolution = "AvoidObsoleteWebServicesInJavaScriptFiles")]
    public class AvoidObsoleteWebServicesInJavaScriptFiles : Rule<JavaScriptFile>
    {
        #region methods

        public override void Visit(JavaScriptFile jsFile, NotificationCollection notifications)
        {
            if (string.IsNullOrEmpty(jsFile.FileContent))
                return;

            string jsScriptContent = jsFile.FileContent;

            SP2007WebServices.AsmxServices.ForEach(asmxService =>
            {
                if (jsScriptContent.IndexOf(asmxService, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Notify(jsFile,
                       string.Format("Avoid referenrring obsolete asmx services in code. Value:[{0}]", asmxService),
                       notifications);
                }
            });

            SP2010WebServices.SvcServices.ForEach(svcService =>
            {
                if (jsScriptContent.IndexOf(svcService, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Notify(jsFile,
                       string.Format("Avoid referenrring obsolete svc services in code. Value:[{0}]", svcService),
                       notifications);
                }
            });
        }

        #endregion
    }
}
