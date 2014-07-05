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
using SPCAFContrib.ReSharper.Common.CodeAnalysis;

[assembly: RegisterConfigurableSeverity(GetPublishingPagesNoParamsHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  GetPublishingPagesNoParamsHighlighting.CheckId + ": " + GetPublishingPagesNoParamsHighlighting.Message,
  "Some recommended practices regarding GetPublishingPages method utilization.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IReferenceExpression) }, HighlightingTypes = new[] { typeof(GetPublishingPagesNoParamsHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class GetPublishingPagesNoParams : ElementProblemAnalyzer<IReferenceExpression>
    {
        protected override void Run(IReferenceExpression element, ElementProblemAnalyzerData analyzerData, IHighlightingConsumer consumer)
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
                            consumer.AddHighlighting(new GetPublishingPagesNoParamsHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IReferenceExpression element)
        {
            IExpressionType expressionType = element.GetExpressionType();

            if (expressionType.IsResolved)
                return element.IsResolvedAsMethodCall(ClrTypeKeys.PublishingWeb,
                    new[]
                    {
                        new MethodCriteria()
                        {
                            ShortName = "GetPublishingPages",
                            Parameters = new ParameterCriteria[0]
                        },
                        new MethodCriteria() {ShortName = "GetPublishingPages", 
                            Parameters = new []
                            {
                                new ParameterCriteria() {ParameterType = typeof(UInt32).FullName, Kind = ParameterKind.VALUE},
                            }}
                    });
            else
            {
                return false;
            }
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class GetPublishingPagesNoParamsHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.GetPublishingPages;
        public const string Message = "Avoid enumerating all PublishingPage objects";

        public IReferenceExpression Element { get; private set; }
        public GetPublishingPagesNoParamsHighlighting(IReferenceExpression element)
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
