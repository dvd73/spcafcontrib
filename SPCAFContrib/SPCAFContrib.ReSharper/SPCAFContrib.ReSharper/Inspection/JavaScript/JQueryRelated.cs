using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Html.Highlightings;
using JetBrains.ReSharper.Daemon.JavaScript.Impl;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Bulk;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Intentions.QuickFixes.Bulk;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.JavaScript.LanguageImpl;
using JetBrains.ReSharper.Psi.JavaScript.Services;
using JetBrains.ReSharper.Psi.JavaScript.Tree;
using JetBrains.ReSharper.Psi.Pointers;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.QuickFix;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.JavaScript;
using IInvocationExpression = JetBrains.ReSharper.Psi.JavaScript.Tree.IInvocationExpression;
using IReferenceExpression = JetBrains.ReSharper.Psi.JavaScript.Tree.IReferenceExpression;

[assembly: RegisterConfigurableSeverity(AvoidJQueryDocumentReadyHighlighting.SeverityId,
  null,
  Consts.CORRECTNESS_GROUP,
  AvoidJQueryDocumentReadyHighlighting.Message,
  "Use _spBodyOnLoadFunctions.push function or SP.SOD.",
  Severity.WARNING,
  false, Internal = false)]

[assembly: RegisterConfigurableSeverity(AvoidDollarGlobalVariableHighlighting.SeverityId,
  null,
  Consts.CORRECTNESS_GROUP,
  AvoidDollarGlobalVariableHighlighting.Message,
  "Use jQuery global variable instead of $.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.JavaScript
{
    /// <summary>
    /// Daemon stage processes work on psi files, and a psi file is an abstraction on physical files. 
    /// Frequently, it’s a one-to-one match, but a physical file can contain more than one psi file. 
    /// For example, html files can also contain JS and CSS files, Razor can contain html, css, js and C#. 
    /// Even C# files can contain snippets of JS and CSS. 
    /// </summary>
    [DaemonStage(StagesBefore = new[] { typeof(LanguageSpecificDaemonStage) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution | 
        IDEProjectType.SPSandbox | 
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SP2013App)]
    public class JQueryRelated : JavaScriptDaemonStageBase
    {
        protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings,
            DaemonProcessKind processKind, IJavaScriptFile file)
        {
            IPsiSourceFile sourceFile = process.SourceFile;
            if (sourceFile.HasExcluded(settings)) return null;

            IProject project = sourceFile.GetProject();
            
            if (project != null)
            {
                if (project.IsApplicableFor(this))
                {
                    return new ResolverProcess(process, settings, file);
                }
            }

            return null;
        }

        public override ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settings)
        {
            return !this.IsSupported(sourceFile) ? ErrorStripeRequest.NONE : ErrorStripeRequest.STRIPE_AND_ERRORS;
        }

        private class ResolverProcess : JavaScriptDaemonStageProcessBase
        {
            private readonly IDaemonProcess _process;
            private readonly IContextBoundSettingsStore _settings;
            private readonly IJavaScriptFile _file;

            public ResolverProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, IJavaScriptFile file) : base(process, settingsStore, file)
            {
                _process = process;
                _settings = settingsStore;
                _file = file;
            }

            protected void HighlightInFile(Action<IJavaScriptFile, IHighlightingConsumer> fileHighlighter, Action<DaemonStageResult> commiter, IContextBoundSettingsStore settingsStore)
            {
                DefaultHighlightingConsumer highlightingConsumer = new DefaultHighlightingConsumer(this, settingsStore);
                fileHighlighter(this._file, highlightingConsumer);
                commiter(new DaemonStageResult(highlightingConsumer.Highlightings));
            }

            public override void Execute(Action<DaemonStageResult> committer)
            {
                if (!DaemonProcess.FullRehighlightingRequired)
                    return;

                HighlightInFile((file, consumer) => file.ProcessThisAndDescendants(this, consumer), committer, _settings);
            }

            public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam, IHighlightingConsumer context)
            {
                if (referenceExpressionParam.Name == "$")
                {
                    context.AddHighlighting(new AvoidDollarGlobalVariableHighlighting(referenceExpressionParam),
                        referenceExpressionParam.GetDocumentRange(), _file);
                }

                base.VisitReferenceExpression(referenceExpressionParam, context);
            }

            public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam, IHighlightingConsumer context)
            {
                if (invocationExpressionParam.InvokedExpression is IReferenceExpression)
                {
                    IReferenceExpression referenceExpression = invocationExpressionParam.InvokedExpression as IReferenceExpression;

                    if ((referenceExpression.Name == "$" || referenceExpression.Name.ToLower() == "jquery") &&
                        invocationExpressionParam.Arguments.Count == 1)
                    {
                        IJavaScriptExpression arg = invocationExpressionParam.Arguments[0];
                        if ((arg is IFunctionExpression) ||
                            (arg is IReferenceExpression && (arg as IReferenceExpression).Name == "document" &&
                             invocationExpressionParam.Parent is IReferenceExpression &&
                             (invocationExpressionParam.Parent as IReferenceExpression).Name == "ready"))
                        {
                            context.AddHighlighting(new AvoidJQueryDocumentReadyHighlighting(invocationExpressionParam),
                                arg is IFunctionExpression
                                    ? referenceExpression.GetDocumentRange()
                                    : invocationExpressionParam.GetDocumentRange(), _file);
                        }
                    }
                }

                base.VisitInvocationExpression(invocationExpressionParam, context);
            }
        }
    }

    // TODO: Нужно предлагать заменять $(document).ready() на _spBodyOnLoadFunctionNames для 2010 или _spBodyOnLoadFunctions для 2013
    [ConfigurableSeverityHighlighting(SeverityId, JavaScriptLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class AvoidJQueryDocumentReadyHighlighting : HtmlWarningHighlighting
    {
        public const string SeverityId = CheckIDs.Rules.ASPXPage.AvoidJQueryDocumentReadyInPage;
        public const string Message = "Avoid using jQuery(document).ready";

        public IJavaScriptTreeNode Element { get; private set; }

        private static string GetCheckId(ProjectFileType fileType, IPsiSourceFile sourceFile)
        {
            string result = String.Empty;

            string fileExt = sourceFile.Name.Split('.').Last().ToLower();
            
            if (fileType is HtmlProjectFileType)
            {
                switch (fileExt)
                {
                    case "aspx":
                        result = CheckIDs.Rules.ASPXPage.AvoidJQueryDocumentReadyInPage;
                        break;
                    case "ascx":
                        result = CheckIDs.Rules.ASCXFile.AvoidJQueryDocumentReadyInControl;
                        break;
                    case "master":
                        result = CheckIDs.Rules.ASPXMasterPage.AvoidJQueryDocumentReadyInMasterPage;
                        break;
                    default:
                        break;
                }
            }
            else if (fileType is JavaScriptProjectFileType)
            {
                result = CheckIDs.Rules.JavaScriptFile.AvoidJQueryDocumentReadyInJSFile;
            }

            return result;
        }

        public AvoidJQueryDocumentReadyHighlighting(IJavaScriptTreeNode element)
            : base(String.Format("{0}: {1}", GetCheckId(element.GetProjectFileType(), element.GetSourceFile()), Message), new object[0])
        {
            this.Element = element;
        }

        public override bool IsValid()
        {
            return this.Element.IsValid();
        }
    }

    [ConfigurableSeverityHighlighting(SeverityId, JavaScriptLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class AvoidDollarGlobalVariableHighlighting : HtmlWarningHighlighting
    {
        public const string SeverityId = CheckIDs.Rules.ASPXPage.AvoidDollarGlobalVariableInPage;
        public const string Message = "Avoid using $ as jQuery reference";

        public IJavaScriptTreeNode Element { get; private set; }

        private static string GetCheckId(ProjectFileType fileType, IPsiSourceFile sourceFile)
        {
            string result = String.Empty;

            string fileExt = sourceFile.Name.Split('.').Last().ToLower();

            if (fileType is HtmlProjectFileType)
            {
                switch (fileExt)
                {
                    case "aspx":
                        result = CheckIDs.Rules.ASPXPage.AvoidDollarGlobalVariableInPage;
                        break;
                    case "ascx":
                        result = CheckIDs.Rules.ASCXFile.AvoidDollarGlobalVariableInControl;
                        break;
                    case "master":
                        result = CheckIDs.Rules.ASPXMasterPage.AvoidDollarGlobalVariableInMasterPage;
                        break;
                    default:
                        break;
                }
            }
            else if (fileType is JavaScriptProjectFileType)
            {
                result = CheckIDs.Rules.JavaScriptFile.AvoidDollarGlobalVariableInJSFile;
            }

            return result;
        }

        public AvoidDollarGlobalVariableHighlighting(IJavaScriptTreeNode element)
            : base(String.Format("{0}: {1}", GetCheckId(element.GetProjectFileType(), element.GetSourceFile()), Message), new object[0])
        {
            this.Element = element;
        }

        public override bool IsValid()
        {
            return this.Element.IsValid();
        }
    }

    [QuickFix]
    public class AvoidDollarGlobalVariableFix : QuickFixBase
    {
        private readonly AvoidDollarGlobalVariableHighlighting _highlighting;
        private const string ACTION_TEXT = "Replace $ elsewhere";
        public AvoidDollarGlobalVariableFix([NotNull] AvoidDollarGlobalVariableHighlighting highlighting)
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
                        IEnumerable<IJavaScriptFile> javascriptFiles =
                            psiFiles.GetPsiFiles<JavaScriptLanguage>(psiSourceFile).OfType<IJavaScriptFile>();
                        foreach (IJavaScriptFile javascriptFile in javascriptFiles)
                        {
                            new AvoidDollarGlobalVariableFileHandler(document, javascriptFile).Run(indicator);
                        }
                    }
                };
            
            var acceptProjectFilePredicate = BulkItentionsBuilderEx.CreateAcceptFilePredicateByPsiLanaguage<JavaScriptLanguage>(solution);
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
                AvoidDollarGlobalVariableFileHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return "Replace '$' with 'jQuery'"; }
        }
    }

    public class AvoidDollarGlobalVariableFileHandler
    {
        private readonly IJavaScriptFile _file;
        private readonly IDocument _document;

        public AvoidDollarGlobalVariableFileHandler(IDocument document, IJavaScriptFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<IReferenceExpression> statements = new List<IReferenceExpression>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IReferenceExpression>(
                s => { if (s.Name == "$") statements.Add(s); }));
            List<ITreeNodePointer<IReferenceExpression>> nodes = statements.Select(x => x.CreateTreeElementPointer()).ToList();

            pi.Start(nodes.Count);
            foreach (ITreeNodePointer<IReferenceExpression> treeNodePointer in nodes)
            {
                IReferenceExpression statement = treeNodePointer.GetTreeNode();
                if (statement != null)
                {
                    Fix(statement, _file.GetSolution());
                    //Logger.LogMessage(LoggingLevel.INFO, "Replaced $ in file " + _file.GetSourceFile().Name);
                }
                pi.Advance(1.0);
            }
        }

        public static void Fix(IJavaScriptTreeNode element, ISolution solution)
        {
            IPsiTransactions transactions = element.GetPsiServices().Transactions;
            JavaScriptElementFactory elementFactory = JavaScriptElementFactory.GetInstance(element);

            transactions.Execute(typeof(AvoidDollarGlobalVariableFileHandler).Name, () =>
            {
                using (solution.GetComponent<IShellLocks>().UsingWriteLock())
                    ModificationUtil.ReplaceChild(element, elementFactory.CreateExpression("jQuery"));
            });
        }
    }

}
