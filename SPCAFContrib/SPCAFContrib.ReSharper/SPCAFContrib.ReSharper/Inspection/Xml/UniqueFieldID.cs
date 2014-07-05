using System;
using System.Linq;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Xml.Highlightings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.DataCache;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.XmlAnalysis;
using SPCAFContrib.ReSharper.Inspection.Xml;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableSeverity(UniqueFieldIDHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  UniqueFieldIDHighlighting.CheckId + ": " + UniqueFieldIDHighlighting.Message,
  "The ID of a field must be unique.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class UniqueFieldID : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    UniqueFieldIDHighlighting errorHighlighting =
                        new UniqueFieldIDHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.Header.ContainerName == "Field")
            {
                if (element.AttributeExists("ID"))
                    result |= CheckElementAttribute(element, "ID");
            }

            return result;
        }

        private static bool CheckElementAttribute(IXmlTag element, string attributeName)
        {
            FieldCache cache = FieldCache.GetInstance(element.GetSolution());
            return cache.GetDuplicates(element, attributeName).Any();
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class UniqueFieldIDHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.FieldTemplate.UniqueFieldID;
        public const string Message = "Do not define duplicate field ID";

        public IXmlTag Element { get; private set; }

        public UniqueFieldIDHighlighting(IXmlTag element) :
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
