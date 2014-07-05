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

[assembly: RegisterConfigurableSeverity(DoNotDeployTaxonomyFieldsInListDefinitionHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DoNotDeployTaxonomyFieldsInListDefinitionHighlighting.CheckId + ": " + DoNotDeployTaxonomyFieldsInListDefinitionHighlighting.Message,
  "Do not deploy taxonomy field using list definition.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DoNotDeployTaxonomyFieldsInListDefinition : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    DoNotDeployTaxonomyFieldsInListDefinitionHighlighting errorHighlighting =
                        new DoNotDeployTaxonomyFieldsInListDefinitionHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.IsFieldDefinition())
            {
                result = element.CheckAttributeValue("Type", "TaxonomyFieldType");
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotDeployTaxonomyFieldsInListDefinitionHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ListTemplate.DoNotDeployTaxonomyFieldsInList;
        public const string Message = "Taxonomy field should be deployed by content type";

        public IXmlTag Element { get; private set; }

        public DoNotDeployTaxonomyFieldsInListDefinitionHighlighting(IXmlTag element) :
            base(String.Format("{0}: {1}", CheckId, Message))
        {
            Element = element;
        }
    }
}
