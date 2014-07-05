using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Xml.Highlightings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.XmlAnalysis;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Xml;

[assembly: RegisterConfigurableSeverity(DeployContentTypesCorrectlyHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DeployContentTypesCorrectlyHighlighting.CheckId + ": " + DeployContentTypesCorrectlyHighlighting.Message,
  "Set of principal checks for content type provision.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DeployContentTypesCorrectly : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    DeployContentTypesCorrectlyHighlighting errorHighlighting =
                        new DeployContentTypesCorrectlyHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.Header.ContainerName == "ContentType")
            {
                bool attOverwriteDeclaredTrue = element.CheckAttributeValue("Overwrite", "true", true);
                bool attInheritsDeclaredFalse = element.CheckAttributeValue("Inherits", "false", true);

                result = attOverwriteDeclaredTrue && (!element.AttributeExists("Inherits") || attInheritsDeclaredFalse);
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DeployContentTypesCorrectlyHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ContentType.DeployContentTypesCorrectly;
        public const string Message = "Do not deploy content type with Overwrite=\"TRUE\" and Inherits=\"False\"(or not specified)";

        public IXmlTag Element { get; private set; }

        public DeployContentTypesCorrectlyHighlighting(IXmlTag element) :
            base(String.Format("{0}: {1}", CheckId, Message))
        {
            Element = element;
        }
    }
}
