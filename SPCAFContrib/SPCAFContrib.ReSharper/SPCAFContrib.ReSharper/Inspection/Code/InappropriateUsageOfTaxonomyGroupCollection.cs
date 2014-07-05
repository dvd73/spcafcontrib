using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(InappropriateUsageOfTaxonomyGroupCollectionHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  InappropriateUsageOfTaxonomyGroupCollectionHighlighting.CheckId + ": " + InappropriateUsageOfTaxonomyGroupCollectionHighlighting.Message,
  "Consider fetching term group or term set by GUID or string comporation by collection enumeration.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IElementAccessExpression) }, HighlightingTypes = new[] { typeof(InappropriateUsageOfTaxonomyGroupCollectionHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class InappropriateUsageOfTaxonomyGroupCollection : ElementProblemAnalyzer<IElementAccessExpression>
    {
        protected override void Run(IElementAccessExpression element, ElementProblemAnalyzerData analyzerData,
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
                                new InappropriateUsageOfTaxonomyGroupCollectionHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IElementAccessExpression element)
        {
            bool result = false;

            IExpressionType expressionType = element.GetExpressionType();

            if (expressionType.IsResolved)
            {
                TreeNodeCollection<ICSharpArgument> arguments = element.Arguments;
                ICSharpArgument firstArgument = arguments.FirstOrDefault();

                if (arguments.IsSingle() && firstArgument.MatchingParameter != null &&
                    firstArgument.MatchingParameter.Element.Type.IsString() &&
                    element.Operand.IsOneOfType(new[]
                    {
                        ClrTypeKeys.TermStoreCollection, 
                        ClrTypeKeys.TermCollection, 
                        ClrTypeKeys.GroupCollection, 
                        ClrTypeKeys.TermSetCollection
                    }))
                {
                    result = true;
                }
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class InappropriateUsageOfTaxonomyGroupCollectionHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.InappropriateUsageOfTaxonomyGroupCollection;
        public const string Message = "Avoid taxonomy collection string based index call";

        public IExpression Element { get; private set; }

        public InappropriateUsageOfTaxonomyGroupCollectionHighlighting(IExpression element)
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
}
