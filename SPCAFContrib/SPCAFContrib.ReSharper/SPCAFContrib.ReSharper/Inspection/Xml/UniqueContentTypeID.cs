using System;
using System.Linq;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Xml.Highlightings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.DataCache;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.XmlAnalysis;
using SPCAFContrib.ReSharper.Inspection.Xml;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableSeverity(UniqueContentTypeIDHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  UniqueContentTypeIDHighlighting.CheckId + ": " + UniqueContentTypeIDHighlighting.Message,
  "The ID of a ContentType must be unique. In some case the id of a content type could be used more than once because of copy-paste errors during development.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class UniqueContentTypeID : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    UniqueContentTypeIDHighlighting errorHighlighting =
                        new UniqueContentTypeIDHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.Header.ContainerName == "ContentType")
            {
                if (element.AttributeExists("ID"))
                    result |= CheckElementAttribute(element, "ID");
            }

            return result;
        }

        private static bool CheckElementAttribute(IXmlTag element, string attributeName)
        {
            ContentTypeCache cache = ContentTypeCache.GetInstance(element.GetSolution());
            return cache.GetDuplicates(element, attributeName).Any();
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class UniqueContentTypeIDHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = "SPC015204";
        public const string Message = "Do not define duplicate content type ID";

        public IXmlTag Element { get; private set; }

        public UniqueContentTypeIDHighlighting(IXmlTag element) :
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
