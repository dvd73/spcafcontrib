using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Xml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Bulk;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Intentions.QuickFixes.Bulk;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Pointers;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Impl.Tree;
using JetBrains.ReSharper.Psi.Xml.Parsing;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.QuickFix;
using SPCAFContrib.ReSharper.Common.XmlAnalysis;
using SPCAFContrib.ReSharper.Inspection.Xml;
using SPCAFContrib.ReSharper.Consts;
using JetBrains.ReSharper.Psi.Xml.Impl.Util;

[assembly: RegisterConfigurableSeverity(DeclareEmptyFieldsElementHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DeclareEmptyFieldsElementHighlighting.CheckId + ": " + DeclareEmptyFieldsElementHighlighting.Message,
  "Declare empty Fields element when using only ContentTypeRefs.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DeclareEmptyFieldsElement : XmlTagProblemAnalyzer
    {
        private bool _contentTypes = false;
        private bool _contentTypeReferences = false;
        private bool _fieldsWrong = false;
        private bool _fieldsMissing = false;

        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    DeclareEmptyFieldsElementHighlighting errorHighlighting = new DeclareEmptyFieldsElementHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public bool IsInvalid(IXmlTag element)
        {
            return !_contentTypes && _contentTypeReferences &&
                   (_fieldsMissing && element.Header.ContainerName == "ContentTypes" ||
                    _fieldsWrong && element.Header.ContainerName == "Fields");
        }

        public override void Init(IXmlFile file)
        {
            base.Init(file);

            _contentTypes = Enumerable.Any(file.GetNestedTags<IXmlTag>("List/MetaData/ContentTypes/ContentType"));
            _contentTypeReferences = Enumerable.Any(file.GetNestedTags<IXmlTag>("List/MetaData/ContentTypes/ContentTypeRef"));
            IXmlTag fieldsTag = file.GetNestedTags<IXmlTag>("List/MetaData/Fields").FirstOrDefault();
            _fieldsWrong = fieldsTag != null && fieldsTag.InnerTags.Any();
            _fieldsMissing = fieldsTag == null;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DeclareEmptyFieldsElementHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ListTemplate.DeclareEmptyFieldsElement;
        public const string Message = "Declare empty Fields element";

        public IXmlTag Element { get; private set; }

        public DeclareEmptyFieldsElementHighlighting(IXmlTag element) :
            base(String.Format("{0}: {1}", CheckId, Message))
        {
            Element = element;
        }
    }

    [QuickFix]
    public class DeclareEmptyFieldsElementFix : QuickFixBase
    {
        private readonly DeclareEmptyFieldsElementHighlighting _highlighting;
        
        public DeclareEmptyFieldsElementFix([NotNull] DeclareEmptyFieldsElementHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public override IEnumerable<IntentionAction> CreateBulbItems()
        {
            ISolution solution = _highlighting.Element.GetSolution();
            IPsiFiles psiFiles = solution.GetComponent<IPsiFiles>();
            IProjectFile projectFile = _highlighting.Element.GetSourceFile().ToProjectFile();

            Action<IDocument, IPsiSourceFile, IProgressIndicator> processFileAction =
                (document, psiSourceFile, indicator) =>
                {
                    if (!psiSourceFile.HasExcluded(psiSourceFile.GetSettingsStore()))
                    {
                        IEnumerable<IXmlFile> xmlFiles =
                            psiFiles.GetPsiFiles<XmlLanguage>(psiSourceFile).OfType<IXmlFile>();
                        foreach (IXmlFile xmlFile in xmlFiles)
                        {
                            new DeclareEmptyFieldsElementFileHandler(document, xmlFile).Run(indicator);
                        }
                    }
                };

            var acceptProjectFilePredicate = BulkItentionsBuilderEx.CreateAcceptFilePredicateByPsiLanaguage<XmlLanguage>(solution);
            var inFileFix = new BulkQuickFixInFileWithCommonPsiTransaction(projectFile, this.Text, processFileAction);
            var builder = new BulkQuickFixWithCommonTransactionBuilder(
                this, inFileFix, solution, this.Text, processFileAction, acceptProjectFilePredicate);

            return builder.CreateBulkActions(projectFile,
              IntentionsAnchors.QuickFixesAnchor, IntentionsAnchors.QuickFixesAnchorPosition);
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return _highlighting.IsValid();
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            if (_highlighting.Element.IsValid())
            {
                DeclareEmptyFieldsElementFileHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return _highlighting.Element.Header.ContainerName == "ContentTypes" ? "Add empty Fields tag" : "Make Fields tag empty"; }
        }
    }

    public class DeclareEmptyFieldsElementFileHandler
    {
        private readonly IXmlFile _file;
        private readonly IDocument _document;
        private DeclareEmptyFieldsElement _rule;

        public DeclareEmptyFieldsElementFileHandler(IDocument document, IXmlFile file)
        {
            this._file = file;
            this._document = document;
            _rule = new DeclareEmptyFieldsElement();
            _rule.Init(_file);
        }

        public void Run(IProgressIndicator pi)
        {
            List<IXmlTag> fields = new List<IXmlTag>();
            

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IXmlTag>(
                element =>
                {
                    if (_rule.IsInvalid(element)) fields.Add(element);
                }));
            List<ITreeNodePointer<IXmlTag>> nodes = fields.Select(x => x.CreateTreeElementPointer()).ToList();

            pi.Start(nodes.Count);
            foreach (ITreeNodePointer<IXmlTag> treeNodePointer in nodes)
            {
                IXmlTag node = treeNodePointer.GetTreeNode();
                if (node != null)
                {
                    Fix(node, _file.GetSolution());
                }
                pi.Advance(1.0);
            }
        }

        public static void Fix(IXmlTag element, ISolution solution)
        {
            XmlElementFactory elementFactory = XmlElementFactory.GetInstance(element);

            if (element.Header.ContainerName == "ContentTypes")
            {
                IXmlTagContainer metadataTag = XmlTagContainerNavigator.GetByTag(element); 
                IXmlTag tagFields = elementFactory.CreateTagForTag(metadataTag as IXmlTag, "<Fields>\r\n</Fields>");
                metadataTag.AddTagAfter(tagFields, element);
            }
            else if (element.Header.ContainerName == "Fields")
            {
                XmlTagUtil.MakeEmptyTag(element);
            }
        }
    }
}
