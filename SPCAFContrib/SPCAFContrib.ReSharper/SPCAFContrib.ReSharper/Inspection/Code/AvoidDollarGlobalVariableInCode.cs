using System;
using System.Text;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(AvoidDollarGlobalVariableInCodeHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  AvoidDollarGlobalVariableInCodeHighlighting.CheckId + ": " + AvoidDollarGlobalVariableInCodeHighlighting.Message,
  "Avoid global $-var as it conflict with assert picker and cmssitemanager.js.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(ILiteralExpression) }, HighlightingTypes = new[] { typeof(AvoidDollarGlobalVariableInCodeHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class AvoidDollarGlobalVariableInCode : ElementProblemAnalyzer<ILiteralExpression>
    {
        protected override void Run(ILiteralExpression element, ElementProblemAnalyzerData analyzerData,
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
                                new AvoidDollarGlobalVariableInCodeHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(ILiteralExpression element)
        {
            bool result = false;

            if (element.ConstantValue.IsString() && element.ConstantValue.Value != null)
            {
                string literal = element.ConstantValue.Value.ToString();
                result = literal.FindJQueryVariableByIndexOf();
            }

            return result;
        }
    }


    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class AvoidDollarGlobalVariableInCodeHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.AvoidDollarGlobalVariableInCode;
        public const string Message = "Avoid using $ as jQuery reference";

        public IExpression Element { get; private set; }

        public AvoidDollarGlobalVariableInCodeHighlighting(IExpression element)
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
