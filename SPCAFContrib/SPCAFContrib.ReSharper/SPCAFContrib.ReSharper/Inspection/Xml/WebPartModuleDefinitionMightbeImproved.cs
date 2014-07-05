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

[assembly: RegisterConfigurableSeverity(WebPartModuleDefinitionMightbeImprovedHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  WebPartModuleDefinitionMightbeImprovedHighlighting.CheckId + ": " + WebPartModuleDefinitionMightbeImprovedHighlighting.Message,
  "Group property value should not be 'Custom'.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class WebPartModuleDefinitionMightbeImproved : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    WebPartModuleDefinitionMightbeImprovedHighlighting errorHighlighting =
                        new WebPartModuleDefinitionMightbeImprovedHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.Header.ContainerName == "Property")
            {
                result = element.CheckAttributeValue("Name", "Group", true) && element.CheckAttributeValue("Value", "Custom", true);
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class WebPartModuleDefinitionMightbeImprovedHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.WebPart.WebPartModuleDefinitionMightbeImproved;
        public const string Message = "Group property value should not be 'Custom'";

        public IXmlTag Element { get; private set; }

        public WebPartModuleDefinitionMightbeImprovedHighlighting(IXmlTag element) :
            base(String.Format("{0}: {1}", CheckId, Message))
        {
            Element = element;
        }

        public override bool IsValid()
        {
            return this.Element.IsValid();
        }
    }
}
