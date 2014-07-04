using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Rules.Other
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
    CheckId = CheckIDs.Rules.Module.AvoidInfoPathForms,
    Help = CheckIDs.Rules.Module.AvoidInfoPathForms_HelpUrl,

    Message = "Avoid InfoPath forms. Detected file:[{0}]",
    DisplayName = "Avoid InfoPath forms",
    Description = "Microsoft is deprecating InfoPath forms in SP2013",
    Resolution = "For the workflow based on SharePoint Designer 2013 and Windows Azure Workflow, SharePoint Designer creates ASPX forms instead of InfoPath forms that were created by SharePoint 2010 workflows. If you want to customize the form, you can click it from the forms slab in workflow summary page to show a generic ASPX page editor.",

    DefaultSeverity = Severity.Warning,
    SharePointVersion = new[] { "15" },
    Links = new []
    {
        "InfoPath dev #1: развертывание InfoPath 2010 форм в составе wsp-решения",
        "http://avishnyakov.wordpress.com/2010/08/25/infopath-dev-1-%D1%80%D0%B0%D0%B7%D0%B2%D0%B5%D1%80%D1%82%D1%8B%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5-infopath-2010-%D1%84%D0%BE%D1%80%D0%BC-%D0%B2-%D1%81%D0%BE%D1%81%D1%82%D0%B0%D0%B2%D0%B5-wsp-%D1%80/",
        "Deploying the Browser enabled InfoPath Form template through feature",
        "http://blogs.msdn.com/b/syedi/archive/2009/06/05/deploying-the-browser-enabled-infopath-form-template-through-feature.aspx"
    }
    )]
    public class AvoidInfoPathForms : Rule<ModuleDefinition>
    {
        #region methods

        public override void Visit(ModuleDefinition module, NotificationCollection notifications)
        {
            IEnumerable<FileDefinition> xsltFiles = module.GetFiles("xsn");

            foreach (FileDefinition file in xsltFiles)
            {
                Notify(file,
                    string.Format(this.MessageTemplate(), file.Path),
                    notifications);
            }
        }

        #endregion
    }
}
