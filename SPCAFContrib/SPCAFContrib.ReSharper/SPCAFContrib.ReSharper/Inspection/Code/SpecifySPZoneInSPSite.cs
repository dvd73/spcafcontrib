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
using JetBrains.ReSharper.Psi.Files;
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
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(SpecifySPZoneInSPSiteHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  SpecifySPZoneInSPSiteHighlighting.CheckId + ": " + SpecifySPZoneInSPSiteHighlighting.Message,
  "Constructor would take default SPUrlZone so that you may have issues with the *.Url properties.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IObjectCreationExpression) }, HighlightingTypes = new[] { typeof(SpecifySPZoneInSPSiteHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class SpecifySPZoneInSPSite : ElementProblemAnalyzer<IObjectCreationExpression>
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
                            consumer.AddHighlighting(new SpecifySPZoneInSPSiteHighlighting(element),
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
            TreeNodeCollection<ICSharpArgument> arguments = element.Arguments;

            if (element.IsOneOfType(new[] { ClrTypeKeys.SPSite }))
            {
                result = arguments.IsSingle();

                if (!result && arguments.Count > 0)
                {
                    // analyse second argument
                    ICSharpArgument p2 = arguments[1];
                    if (p2.MatchingParameter != null)
                    {
                        result = !p2.MatchingParameter.Element.IsOneOfTheTypes(new[] {ClrTypeKeys.SPUrlZone});
                    }
                }
            }
            
            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class SpecifySPZoneInSPSiteHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.SpecifySPZoneInSPSite;
        public const string Message = "Missing SPUrlZone parameter in SPSite constructor";

        public IObjectCreationExpression Element { get; private set; }
        public SpecifySPZoneInSPSiteHighlighting(IObjectCreationExpression element)
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
    public class SpecifySPZoneInSPSiteFix : QuickFixBase
    {
        private readonly SpecifySPZoneInSPSiteHighlighting _highlighting;
        private const string ACTION_TEXT = "Insert SPContext.Current.Site.Zone as second argument";

        public SpecifySPZoneInSPSiteFix([NotNull] SpecifySPZoneInSPSiteHighlighting highlighting)
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
                            new SpecifySPZoneInSPSiteFileHandler(document, csharpFile).Run(indicator);
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
                SpecifySPZoneInSPSiteFileHandler.Fix(_highlighting.Element, solution);
            }

            return null;
        }

        public override string Text
        {
            get { return ACTION_TEXT; }
        }
    }

    public class SpecifySPZoneInSPSiteFileHandler
    {
        private readonly ICSharpFile _file;
        private readonly IDocument _document;

        public SpecifySPZoneInSPSiteFileHandler(IDocument document, ICSharpFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<IObjectCreationExpression> statements = new List<IObjectCreationExpression>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IObjectCreationExpression>(
                s => { if (SpecifySPZoneInSPSite.IsInvalid(s)) statements.Add(s); }));
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
            IPsiTransactions transactions = element.GetPsiServices().Transactions;
            CSharpElementFactory elementFactory = CSharpElementFactory.GetInstance(element);
            TreeNodeCollection<ICSharpArgument> arguments = element.Arguments;
            IReferenceExpression referenceExpression =
                elementFactory.CreateReferenceExpression("SPContext.Current.Site.Zone", new object());
            ICSharpArgument p1 = arguments[0];
            ICSharpArgument p2 = elementFactory.CreateArgument(ParameterKind.VALUE, referenceExpression);
            element.AddArgumentAfter(p2, p1);
        }
    }
}
