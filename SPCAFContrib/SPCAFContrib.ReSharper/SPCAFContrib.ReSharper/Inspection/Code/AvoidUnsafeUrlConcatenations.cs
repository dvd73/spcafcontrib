using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Bulk;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Intentions.QuickFixes.Bulk;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.Resolve;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
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
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(AvoidUnsafeUrlConcatenationsHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  AvoidUnsafeUrlConcatenationsHighlighting.CheckId + ": " + AvoidUnsafeUrlConcatenationsHighlighting.Message,
  "Url property for SPSite, SPWeb and SPFolder may return string with or without triling slash.",
  Severity.WARNING,
  false, Internal = false)]


namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IAdditiveExpression) }, HighlightingTypes = new[] { typeof(AvoidUnsafeUrlConcatenationsHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class AvoidUnsafeUrlConcatenations : ElementProblemAnalyzer<IAdditiveExpression>
    {
        protected override void Run(IAdditiveExpression element, ElementProblemAnalyzerData analyzerData,
            IHighlightingConsumer consumer)
        {
            IPsiSourceFile sourceFile = element.GetSourceFile();

            if (sourceFile != null)
            {
                if (sourceFile.HasExcluded(analyzerData.SettingsStore)) return;

                IProject project = sourceFile.GetProject();

                if (project != null)
                {
                    if (project.IsApplicableFor(this))
                    {
                        if (IsInvalid(element))
                        {
                            consumer.AddHighlighting(
                                new AvoidUnsafeUrlConcatenationsHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IAdditiveExpression element)
        {
            return
                element.Arguments.Any(
                    argument =>
                        argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPWeb, new[] {"Url", "ServerRelativeUrl"}) ||
                        argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPSite, new[] {"Url", "ServerRelativeUrl"}) ||
                        argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPFolder, new[] {"Url", "ServerRelativeUrl"}));
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.WARNING, ShowToolTipInStatusBar = true)]
    public class AvoidUnsafeUrlConcatenationsHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.AvoidUnsafeUrlConcatenations;
        public const string Message = "Avoid unsafe url concatenation";

        public IAdditiveExpression Element { get; private set; }

        public AvoidUnsafeUrlConcatenationsHighlighting(IAdditiveExpression element)
        {
            this.Element = element;
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
            return this.Element != null && this.Element.IsValid();
        }

        #endregion
    }

    [QuickFix]
    public class AvoidUnsafeUrlConcatenationsFix : QuickFixBase
    {
        private readonly AvoidUnsafeUrlConcatenationsHighlighting _highlighting;
        private const string ACTION_TEXT = "Replace by SPUrlUtility.CombineUrl()";

        public AvoidUnsafeUrlConcatenationsFix([NotNull] AvoidUnsafeUrlConcatenationsHighlighting highlighting)
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
                        IEnumerable<ICSharpFile> csharpFiles =
                            psiFiles.GetPsiFiles<CSharpLanguage>(psiSourceFile).OfType<ICSharpFile>();
                        foreach (ICSharpFile csharpFile in csharpFiles)
                        {
                            new AvoidUnsafeUrlConcatenationsFileHandler(document, csharpFile).Run(indicator);
                        }
                    }
                };

            var acceptProjectFilePredicate =
                BulkItentionsBuilderEx.CreateAcceptFilePredicateByPsiLanaguage<CSharpLanguage>(solution);
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
                AvoidUnsafeUrlConcatenationsFileHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return ACTION_TEXT; }
        }
    }

    public class AvoidUnsafeUrlConcatenationsFileHandler
    {
        private readonly ICSharpFile _file;
        private readonly IDocument _document;

        public AvoidUnsafeUrlConcatenationsFileHandler(IDocument document, ICSharpFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<IAdditiveExpression> statements = new List<IAdditiveExpression>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IAdditiveExpression>(
                s => { if (AvoidUnsafeUrlConcatenations.IsInvalid(s)) statements.Add(s); }));
            List<ITreeNodePointer<IAdditiveExpression>> nodes = statements.Select(x => x.CreateTreeElementPointer()).ToList();

            pi.Start(nodes.Count);
            foreach (ITreeNodePointer<IAdditiveExpression> treeNodePointer in nodes)
            {
                IAdditiveExpression statement = treeNodePointer.GetTreeNode();
                if (statement != null)
                {
                    Fix(statement, _file.GetSolution());
                }
                pi.Advance(1.0);
            }
        }

        public static void Fix(IAdditiveExpression element, ISolution solution)
        {
            const string namespaceIdentifier = "Microsoft.SharePoint.Utilities";
            string expressionFormat = "SPUrlUtility.CombineUrl({0}, {1})";

            var file = element.GetContainingFile() as ICSharpFile;
            var elementFactory = CSharpElementFactory.GetInstance(element);

            if (!file.Imports.Any(d => d.ImportedSymbolName.QualifiedName.Equals(namespaceIdentifier)))
                file.AddImport(elementFactory.CreateUsingDirective(namespaceIdentifier));

            expressionFormat = String.Format(expressionFormat, GetArgumentValue(element.Arguments[0]),
                GetArgumentValue(element.Arguments[1]));
            ICSharpExpression referenceExpression = elementFactory.CreateExpressionAsIs(expressionFormat);
            element.ReplaceBy(referenceExpression);
        }

        private static string GetArgumentValue(ICSharpArgumentInfo argument)
        {
            if (argument is ExpressionArgumentInfo)
                return (argument as ExpressionArgumentInfo).Expression.GetText();
            
            return "\"\"";
        }
    }
}
