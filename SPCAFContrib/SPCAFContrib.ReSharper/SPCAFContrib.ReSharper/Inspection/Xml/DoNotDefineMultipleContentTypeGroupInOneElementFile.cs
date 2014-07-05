using System;
using System.Collections.Generic;
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
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.XmlAnalysis;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Xml;

[assembly: RegisterConfigurableSeverity(DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting.CheckId + ": " + DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting.Message,
  "Do not define multiple content type groups in one element file.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DoNotDefineMultipleContentTypeGroupInOneElementFile : XmlTagProblemAnalyzer
    {
        private bool _moreThenOneNames = false;
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting errorHighlighting =
                        new DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public override void Init(IXmlFile file)
        {
            base.Init(file);

            IList<IXmlTag> tags = file.GetNestedTags<IXmlTag>("Elements/ContentType");
            _moreThenOneNames = tags.Where(t => t.AttributeExists("Group")).Select(t => t.GetAttribute("Group").UnquotedValue).Distinct().Count() > 1;
        }

        public bool IsInvalid(IXmlTag element)
        {
            return element.Header.ContainerName == "ContentType" && _moreThenOneNames;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ContentType.DoNotDefineMultipleContentTypeGroupInOneElementFile;
        public const string Message = "Do not define multiple content type groups in one element file";

        public IXmlTag Element { get; private set; }

        public DoNotDefineMultipleContentTypeGroupInOneElementFileHighlighting(IXmlTag element) :
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
