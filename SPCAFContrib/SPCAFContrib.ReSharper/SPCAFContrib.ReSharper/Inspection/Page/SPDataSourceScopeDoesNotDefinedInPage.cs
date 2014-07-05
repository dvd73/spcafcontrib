using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Asp.Stages;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Bulk;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Intentions.QuickFixes.Bulk;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp;
using JetBrains.ReSharper.Psi.Asp.Parsing;
using JetBrains.ReSharper.Psi.Asp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Html.Tree;
using JetBrains.ReSharper.Psi.Pointers;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.QuickFix;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Page;
using JetBrains.Annotations;

[assembly: RegisterConfigurableSeverity(SPDataSourceScopeDoesNotDefinedInPageHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  SPDataSourceScopeDoesNotDefinedInPageHighlighting.CheckId + ": " + SPDataSourceScopeDoesNotDefinedInPageHighlighting.Message,
  "All SPViewScope enumeration values are covered all possible developer's intentions. If not specified SharePoint will use Default value.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Page
{
    [DaemonStage(StagesBefore = new[] { typeof(LanguageSpecificDaemonStage) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class SPDataSourceScopeDoesNotDefinedInPage : AspDaemonStageBase
    {
        protected override IDaemonStageProcess CreateProcessInternal(IAspFile file, IDaemonProcess process,
            IContextBoundSettingsStore settings)
        {
            IPsiSourceFile sourceFile = process.SourceFile;
            if (sourceFile.HasExcluded(settings)) return null;

            IProject project = sourceFile.GetProject();

            if (project != null)
            {
                if (project.IsApplicableFor(this))
                {
                    return new ResolverProcess(process, file, settings);
                }
            }

            return null;
        }

        public static bool IsInvalid(IAspTag tag)
        {
            return tag.IsRunatServer &&
                   tag.TagType == HtmlTagType.CUSTOM_CONTROL &&
                   tag.TagName == "SPDataSource" && !tag.AttributeExists("Scope");
        }

        private class ResolverProcess : HtmlTreeVisitor<IHighlightingConsumer, bool>, IRecursiveElementProcessor<IHighlightingConsumer>, IDaemonStageProcess
        {
            private readonly IAspFile _file;
            private readonly IDaemonProcess _process;
            private readonly IContextBoundSettingsStore _settings;

            public IDaemonProcess DaemonProcess
            {
                get
                {
                    return _process;
                }
            }

            public ResolverProcess(IDaemonProcess process, IAspFile file, IContextBoundSettingsStore settings) 
            {
                _file = file;
                _process = process;
                _settings = settings;
            }

            public void Execute(Action<DaemonStageResult> commiter)
            {
                if (!DaemonProcess.FullRehighlightingRequired)
                    return;

                HighlightInFile((file, consumer) => _file.ProcessThisAndDescendants(this, consumer), commiter, _settings);
            }

            protected void HighlightInFile(Action<IAspFile, IHighlightingConsumer> fileHighlighter, Action<DaemonStageResult> commiter, IContextBoundSettingsStore settingsStore)
            {
                DefaultHighlightingConsumer highlightingConsumer = new DefaultHighlightingConsumer(this, settingsStore);
                fileHighlighter(_file, highlightingConsumer);
                commiter(new DaemonStageResult(highlightingConsumer.Highlightings));
            }

            public virtual bool InteriorShouldBeProcessed(ITreeNode element, IHighlightingConsumer context)
            {
                return element is IAspFile || element is IAspTag;
            }

            public bool IsProcessingFinished(IHighlightingConsumer context)
            {
                if (DaemonProcess.InterruptFlag)
                    throw new ProcessCancelledException();
                else
                    return false;
            }

            public virtual void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer consumer)
            {
            }

            public virtual void ProcessAfterInterior(ITreeNode element, IHighlightingConsumer consumer)
            {
                IHtmlTreeNode aspTreeNode = element as IHtmlTreeNode;
                if (aspTreeNode != null)
                {
                    ITokenNode tokenNode = aspTreeNode as ITokenNode;
                    if (tokenNode != null && tokenNode.GetTokenType().IsWhitespace)
                        return;
                    aspTreeNode.AcceptVisitor(this, consumer);
                }
                else
                    VisitNode(element, consumer);
            }

            public override bool VisitHtmlTag(IHtmlTag tag, IHighlightingConsumer consumer)
            {
                if (tag is IAspTag)
                {
                    IAspTag aspTag = tag as IAspTag;

                    if (IsInvalid(aspTag))
                    {
                        consumer.AddHighlighting(new SPDataSourceScopeDoesNotDefinedInPageHighlighting(aspTag), _file);
                        return false;
                    }
                }

                return base.VisitHtmlTag(tag, consumer);
            }

        }

    }

    [ConfigurableSeverityHighlighting(CheckId, AspLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class SPDataSourceScopeDoesNotDefinedInPageHighlighting : IHighlightingWithRange
    {
        public const string CheckId = CheckIDs.Rules.ASPXPage.SPDataSourceScopeDoesNotDefinedInPage;
        public const string Message = "SPDataSource Scope is not defined";
        public IAspTag Element { get; private set; }

        public SPDataSourceScopeDoesNotDefinedInPageHighlighting(IAspTag element)
        {
            Element = element;
        }

        #region IHighlighting Members

        public string ToolTip
        {
            get
            {
                return String.Format("{0}: {1}", CheckId, Message);
            }
        }

        public string ErrorStripeToolTip
        {
            get { return ToolTip; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

        public bool IsValid()
        {
            return Element != null && Element.IsValid();
        }

        #endregion

        #region IHighlightingWithRange Members

        public DocumentRange CalculateRange()
        {
            return Element.GetNavigationRange();
        }

        #endregion
    }

    [QuickFix]
    public class SPDataSourceScopeDoesNotDefinedInPageFix : QuickFixBase
    {
        private readonly SPDataSourceScopeDoesNotDefinedInPageHighlighting _highlighting;
        private const string ACTION_TEXT = "Specify Scope as SPViewScope.Recursive";

        public SPDataSourceScopeDoesNotDefinedInPageFix([NotNull] SPDataSourceScopeDoesNotDefinedInPageHighlighting highlighting)
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
                        IEnumerable<IAspFile> csharpFiles =
                            psiFiles.GetPsiFiles<AspLanguage>(psiSourceFile).OfType<IAspFile>();
                        foreach (IAspFile csharpFile in csharpFiles)
                        {
                            new SPDataSourceScopeDoesNotDefinedInPageHandler(document, csharpFile).Run(indicator);
                        }
                    }
                };

            var acceptProjectFilePredicate =
                BulkItentionsBuilderEx.CreateAcceptFilePredicateByPsiLanaguage<AspLanguage>(solution);
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
                SPDataSourceScopeDoesNotDefinedInPageHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return ACTION_TEXT; }
        }
    }

    public class SPDataSourceScopeDoesNotDefinedInPageHandler
    {
        private readonly IAspFile _file;
        private readonly IDocument _document;

        public SPDataSourceScopeDoesNotDefinedInPageHandler(IDocument document, IAspFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<IAspTag> statements = new List<IAspTag>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IAspTag>(
                s => { if (SPDataSourceScopeDoesNotDefinedInPage.IsInvalid(s)) statements.Add(s); }));
            List<ITreeNodePointer<IAspTag>> nodes = statements.Select(x => x.CreateTreeElementPointer()).ToList();

            pi.Start(nodes.Count);
            foreach (ITreeNodePointer<IAspTag> treeNodePointer in nodes)
            {
                IAspTag statement = treeNodePointer.GetTreeNode();
                if (statement != null)
                {
                    Fix(statement, _file.GetSolution());
                }
                pi.Advance(1.0);
            }
        }

        public static void Fix(IAspTag element, ISolution solution)
        {
            element.AddAttribute("Scope", "Recursive");
        }
    }
}
