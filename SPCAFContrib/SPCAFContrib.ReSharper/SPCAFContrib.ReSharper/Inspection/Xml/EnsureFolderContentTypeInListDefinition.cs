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
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Xml;

[assembly: RegisterConfigurableSeverity(EnsureFolderContentTypeInListDefinitionHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  EnsureFolderContentTypeInListDefinitionHighlighting.CheckId + ": " + EnsureFolderContentTypeInListDefinitionHighlighting.Message,
  "Ensure Folder ContentTypeRef in list definition.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class EnsureFolderContentTypeInListDefinition : XmlTagProblemAnalyzer
    {
        private bool _hasFolderContentType = false;
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    EnsureFolderContentTypeInListDefinitionHighlighting errorHighlighting =
                        new EnsureFolderContentTypeInListDefinitionHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public override void Init(IXmlFile file)
        {
            base.Init(file);

            _hasFolderContentType = file.GetNestedTags<IXmlTag>("List/MetaData/ContentTypes/ContentTypeRef").Any(t => t.CheckAttributeValue("ID", "0x0120"));
        }

        public bool IsInvalid(IXmlTag element)
        {
            return element.Header.ContainerName == "ContentTypes" && !_hasFolderContentType;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class EnsureFolderContentTypeInListDefinitionHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.ListTemplate.EnsureFolderContentTypeInListDefinition;
        public const string Message = "List Definition should include Folder ContentTypeRef";

        public IXmlTag Element { get; private set; }

        public EnsureFolderContentTypeInListDefinitionHighlighting(IXmlTag element) :
            base(String.Format("{0}: {1}", CheckId, Message))
        {
            Element = element;
        }

        public override bool IsValid()
        {
            return this.Element.IsValid();
        }
    }

    [QuickFix]
    public class EnsureFolderContentTypeInListDefinitionFix : QuickFixBase
    {
        private readonly EnsureFolderContentTypeInListDefinitionHighlighting _highlighting;
        private const string ACTION_TEXT = "Add ContentTypeRef with ID=\"0x0120\"";
        public EnsureFolderContentTypeInListDefinitionFix([NotNull] EnsureFolderContentTypeInListDefinitionHighlighting highlighting)
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
                            new EnsureFolderContentTypeInListDefinitionFileHandler(document, xmlFile).Run(indicator);
                        }
                    }
                };

            var acceptProjectFilePredicate = BulkItentionsBuilderEx.CreateAcceptFilePredicateByPsiLanaguage<XmlLanguage>(solution);
            var inFileFix = new BulkQuickFixInFileWithCommonPsiTransaction(projectFile, ACTION_TEXT, processFileAction);
            var builder = new BulkQuickFixWithCommonTransactionBuilder(
                this, inFileFix, solution, ACTION_TEXT, processFileAction, acceptProjectFilePredicate);

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
                EnsureFolderContentTypeInListDefinitionFileHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return ACTION_TEXT; }
        }
    }

    public class EnsureFolderContentTypeInListDefinitionFileHandler
    {
        private readonly IXmlFile _file;
        private readonly IDocument _document;
        private readonly EnsureFolderContentTypeInListDefinition _rule;

        public EnsureFolderContentTypeInListDefinitionFileHandler(IDocument document, IXmlFile file)
        {
            this._file = file;
            this._document = document;
            this._rule = new EnsureFolderContentTypeInListDefinition();
            this._rule.Init(file);
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
            
            IXmlTag tagFolderCT = elementFactory.CreateTagForTag(element, "<ContentTypeRef ID=\"0x0120\" />");
            element.AddTagAfter(tagFolderCT, null);
        }
    }
}
