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

[assembly: RegisterConfigurableSeverity(DoNotAllowDeletionForFieldHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  DoNotAllowDeletionForFieldHighlighting.CheckId + ": " + DoNotAllowDeletionForFieldHighlighting.Message,
  "Prevent SharePoint field from deletion.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DoNotAllowDeletionForField : XmlTagProblemAnalyzer
    {
        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                if (IsInvalid(element))
                {
                    DoNotAllowDeletionForFieldHighlighting errorHighlighting = new DoNotAllowDeletionForFieldHighlighting(element);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static bool IsInvalid(IXmlTag element)
        {
            bool result = false;

            if (element.IsFieldDefinition())
            {
                result = !element.AttributeExists("AllowDeletion");
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotAllowDeletionForFieldHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.FieldTemplate.DoNotAllowDeletionForField;
        public const string Message = "Add AllowDeletion=\"FALSE\" attribute";

        public IXmlTag Element { get; private set; }

        public DoNotAllowDeletionForFieldHighlighting(IXmlTag element) :
            base(String.Format("{0}: {1}", CheckId, Message))
        {
            Element = element;
        }
    }

    [QuickFix]
    public class DoNotAllowDeletionForFieldFix : QuickFixBase
    {
        private readonly DoNotAllowDeletionForFieldHighlighting _highlighting;
        private const string ACTION_TEXT = "Add AllowDeletion=\"FALSE\" attribute";
        public DoNotAllowDeletionForFieldFix([NotNull] DoNotAllowDeletionForFieldHighlighting highlighting)
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
                            new DoNotAllowDeletionForFieldFileHandler(document, xmlFile).Run(indicator);
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
                DoNotAllowDeletionForFieldFileHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return ACTION_TEXT; }
        }
    }

    public class DoNotAllowDeletionForFieldFileHandler
    {
        private readonly IXmlFile _file;
        private readonly IDocument _document;

        public DoNotAllowDeletionForFieldFileHandler(IDocument document, IXmlFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<IXmlTag> fields = new List<IXmlTag>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IXmlTag>(
                element =>
                {
                    if (DoNotAllowDeletionForField.IsInvalid(element)) fields.Add(element);
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
            element.EnsureAttribute("AllowDeletion", "FALSE");
        }
    }
}
