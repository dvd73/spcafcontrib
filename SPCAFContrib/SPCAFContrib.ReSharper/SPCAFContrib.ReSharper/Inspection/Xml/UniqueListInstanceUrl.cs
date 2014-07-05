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

[assembly: RegisterConfigurableSeverity(UniqueListInstanceUrlHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  UniqueListInstanceUrlHighlighting.CheckId + ": " + UniqueListInstanceUrlHighlighting.Message,
  "Not unique list instance Url might lead to the fail.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class UniqueListInstanceUrl : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    UniqueListInstanceUrlHighlighting errorHighlighting =
                        new UniqueListInstanceUrlHighlighting(element);
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

        private static bool CheckElementAttribute(IXmlTag element, string attributeName)
        {
            ListInstanceCache cache = ListInstanceCache.GetInstance(element.GetSolution());
            return cache.GetDuplicates(element, attributeName).Any();
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class UniqueListInstanceUrlHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ListInstance.UniqueListInstanceUrl;
        public const string Message = "Do not define duplicate list instance Url";

        public IXmlTag Element { get; private set; }

        public UniqueListInstanceUrlHighlighting(IXmlTag element) :
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
