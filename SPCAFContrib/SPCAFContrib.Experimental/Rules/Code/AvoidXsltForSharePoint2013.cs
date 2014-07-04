using System.Collections.Generic;
using SPCAF.Sdk;
using SPCAF.Sdk.Model;
using SPCAF.Sdk.Rules;
using SPCAFContrib.Consts;
using SPCAFContrib.Extensions;

namespace SPCAFContrib.Experimental.Rules.Code
{
    [RuleMetadata(typeof(ContribCorrectnessGroup),
    CheckId = CheckIDs.Rules.Module.AvoidXsltForSharePoint2013,
    DisplayName = "AvoidXsltForSharePoint2013",
    Description = "AvoidXsltForSharePoint2013",
    DefaultSeverity = Severity.Warning,
    SharePointVersion = new[] { "15" },
    Message = "AvoidXsltForSharePoint2013",
    Resolution = "AvoidXsltForSharePoint2013")]
    // TODO: add check through feature
    // TODO: make property to exclude particular files. Will sort out the default set of the XSLT file which are fine to use. 
    // TODO: Есть несколько вполне валидных кейсов для XSL в 2013: data form web part, server-side rendering для CBS, шаблоны для CQWP.
    // TODO: provision xml файлов, кастомизирущих представления(SPView) списков. Это может банально приводить к ошибкам если копипастишь куски решений из проектов 2010 в 2013. Да и вообще немного смысла делать представления списков в 2013 на XSL. Надо не xsl файлы проверять, а параметры view
    public class AvoidXsltForSharePoint2013 : Rule<ModuleDefinition>
    {
        #region methods

        public override void Visit(ModuleDefinition module, NotificationCollection notifications)
        {
            IEnumerable<FileDefinition> xsltFiles = module.GetFiles("xsl");

            foreach (FileDefinition file in xsltFiles)
            {
                Notify(file,
                    string.Format("Avoid xslt files in SharePoint 2013. Detected file:[{0}]", file.Path),
                    notifications);
            }
        }

        #endregion
    }
}
