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

[assembly: RegisterConfigurableSeverity(DoNotUseSystemListNamesHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DoNotUseSystemListNamesHighlighting.CheckId + ": " + DoNotUseSystemListNamesHighlighting.Message,
  "Do not use reserved url for the list instance.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DoNotUseSystemListNames : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    DoNotUseSystemListNamesHighlighting errorHighlighting =
                        new DoNotUseSystemListNamesHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.Header.ContainerName == "ListInstance")
            {
                if (element.AttributeExists("Url"))
                    result |= CheckElementAttribute(element, "Url");
            }

            return result;
        }

        private static bool CheckElementAttribute(IXmlTag element, string attName)
        {
            IXmlAttribute attribute = element.GetAttribute(attName);
            return SharePointOOBListInstances.ListInstances.Exists(z => z.Url.Equals(attribute.UnquotedValue));
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotUseSystemListNamesHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ListInstance.DoNotUseSystemListNames;
        public const string Message = "Do not use out of the box list urls";

        public IXmlTag Element { get; private set; }

        public DoNotUseSystemListNamesHighlighting(IXmlTag element) :
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
