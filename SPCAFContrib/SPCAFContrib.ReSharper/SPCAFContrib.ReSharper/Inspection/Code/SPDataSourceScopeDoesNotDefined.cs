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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
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

[assembly: RegisterConfigurableSeverity(SPDataSourceScopeDoesNotDefinedHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  SPDataSourceScopeDoesNotDefinedHighlighting.CheckId + ": " + SPDataSourceScopeDoesNotDefinedHighlighting.Message,
  "All SPViewScope enumeration values are covered all possible developer's intentions. If not specified SharePoint will use Default value.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IObjectCreationExpression) }, HighlightingTypes = new[] { typeof(SPDataSourceScopeDoesNotDefinedHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class SPDataSourceScopeDoesNotDefined : ElementProblemAnalyzer<IObjectCreationExpression>
    {
        protected override void Run(IObjectCreationExpression element, ElementProblemAnalyzerData analyzerData, IHighlightingConsumer consumer)
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
                            consumer.AddHighlighting(new SPDataSourceScopeDoesNotDefinedHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IObjectCreationExpression element)
        {
            bool result = false;

            if (element.IsOneOfType(new[] { ClrTypeKeys.SPDataSource }))
            {
                ICSharpTypeMemberDeclaration method = element.GetContainingTypeMemberDeclaration();
                ILocalVariableDeclaration variable = element.GetContainingNode<ILocalVariableDeclaration>(false);
                bool inInitializer = false;

                if (element.Initializer != null)
                {
                    inInitializer = element.Initializer.InitializerElements.Any(
                        initializerElement =>
                            initializerElement is IMemberInitializer && (initializerElement as IMemberInitializer).NameIdentifier.Name == "Scope");
                }

                if (!inInitializer && variable != null)
                {
                    string varName = variable.DeclaredElement.ShortName;
                    result = !method.HasPropertySet(ClrTypeKeys.SPDataSource, "Scope", varName);
                }
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class SPDataSourceScopeDoesNotDefinedHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.SPDataSourceScopeDoesNotDefined;
        public const string Message = "SPDataSource scope is not defined";

        public IObjectCreationExpression Element { get; private set; }
        public SPDataSourceScopeDoesNotDefinedHighlighting(IObjectCreationExpression element)
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
    public class SPDataSourceScopeDoesNotDefinedFix : QuickFixBase
    {
        private readonly SPDataSourceScopeDoesNotDefinedHighlighting _highlighting;
        private const string ACTION_TEXT = "Specify Scope as SPViewScope.Recursive";

        public SPDataSourceScopeDoesNotDefinedFix([NotNull] SPDataSourceScopeDoesNotDefinedHighlighting highlighting)
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
                            new SPDataSourceScopeDoesNotDefinedHandler(document, csharpFile).Run(indicator);
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
                SPDataSourceScopeDoesNotDefinedHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return ACTION_TEXT; }
        }
    }

    public class SPDataSourceScopeDoesNotDefinedHandler
    {
        private readonly ICSharpFile _file;
        private readonly IDocument _document;

        public SPDataSourceScopeDoesNotDefinedHandler(IDocument document, ICSharpFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<IObjectCreationExpression> statements = new List<IObjectCreationExpression>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IObjectCreationExpression>(
                s => { if (SPDataSourceScopeDoesNotDefined.IsInvalid(s)) statements.Add(s); }));
            List<ITreeNodePointer<IObjectCreationExpression>> nodes = statements.Select(x => x.CreateTreeElementPointer()).ToList();

            pi.Start(nodes.Count);
            foreach (ITreeNodePointer<IObjectCreationExpression> treeNodePointer in nodes)
            {
                IObjectCreationExpression statement = treeNodePointer.GetTreeNode();
                if (statement != null)
                {
                    Fix(statement, _file.GetSolution());
                }
                pi.Advance(1.0);
            }
        }

        public static void Fix(IObjectCreationExpression element, ISolution solution)
        {
            string expressionFormat = "{0}.Scope = SPViewScope.Recursive;";
            var elementFactory = CSharpElementFactory.GetInstance(element);

            ILocalVariableDeclaration variable = element.GetContainingNode<ILocalVariableDeclaration>(false);

            if (variable != null)
            {
                string varName = variable.DeclaredElement.ShortName;
                expressionFormat = String.Format(expressionFormat, varName);
                ICSharpStatement containingStatement = element.GetContainingStatement();

                var newStatement = elementFactory.CreateStatement(expressionFormat, new object());
                if (containingStatement != null)
                    ModificationUtil.AddChildAfter(containingStatement, newStatement);
            }
        }
    }
}
