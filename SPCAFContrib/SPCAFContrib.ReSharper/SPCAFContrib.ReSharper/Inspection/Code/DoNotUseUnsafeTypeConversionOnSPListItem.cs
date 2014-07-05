using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(DoNotUseUnsafeTypeConversionOnSPListItemHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DoNotUseUnsafeTypeConversionOnSPListItemHighlighting.CheckId + ": " + DoNotUseUnsafeTypeConversionOnSPListItemHighlighting.Message,
  "SPListItem is untyped entity, so null reference exceptions on nullable types or wrong type conversion exception might arise.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IElementAccessExpression) }, HighlightingTypes = new[] { typeof(DoNotUseUnsafeTypeConversionOnSPListItemHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class DoNotUseUnsafeTypeConversionOnSPListItem : ElementProblemAnalyzer<IElementAccessExpression>
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
                                new DoNotUseUnsafeTypeConversionOnSPListItemHighlighting(element),
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
                if (element.Operand.GetExpressionType().ToString() == TypeKeys.SPListItem)
                {
                    int i = 0;

                    bool dotFound = false;
                    bool toStringFound = false;
                    foreach (ITreeNode node in element.RightSiblings())
                    {
                        if (i == 0 && node.NodeType.ToString() == "DOT")
                        {
                            dotFound = true;
                        }

                        if (i == 1 && node.NodeType.ToString() == "IDENTIFIER" &&
                            node.Parent is IReferenceExpression)
                        {
                            if ((node.Parent as IReferenceExpression).NameIdentifier.Name == "ToString")
                                toStringFound = true;
                        }

                        i++;
                        if (i > 1) break;
                    }

                    bool rparenthFound = false;
                    bool lparenthFound = false;
                    bool usertypeUsage = false;
                    foreach (ITreeNode node in element.LeftSiblings())
                    {
                        if (node.IsWhitespaceToken()) continue;

                        if (i == 0 && node.NodeType.ToString() == "RPARENTH")
                        {
                            rparenthFound = true;
                        }

                        if (i == 1 && node.NodeType.ToString() == "USER_TYPE_USAGE")
                        {
                            usertypeUsage = true;
                        }

                        if (i == 2 && node.NodeType.ToString() == "LPARENTH")
                        {
                            lparenthFound = true;
                        }

                        i++;
                        if (i > 2) break;
                    }

                    if (dotFound && toStringFound ||
                        rparenthFound && lparenthFound && usertypeUsage)
                        result = true;
                }
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotUseUnsafeTypeConversionOnSPListItemHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.DoNotUseUnsafeTypeConversionOnSPListItem;
        public const string Message = "Do not use unsafe casts on SPListItem";

        public IExpression Element { get; private set; }

        public DoNotUseUnsafeTypeConversionOnSPListItemHighlighting(IExpression element)
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
