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

[assembly: RegisterConfigurableSeverity(UseSecureStoreServiceHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  UseSecureStoreServiceHighlighting.CheckId + ": " + UseSecureStoreServiceHighlighting.Message,
  "At some point, it is better to use Secure Store Provider to store credentials/connection strins intead of saving them into web.config, property bags or lists.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IObjectCreationExpression) }, HighlightingTypes = new[] { typeof(UseSecureStoreServiceHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class UseSecureStoreService : ElementProblemAnalyzer<IObjectCreationExpression>
    {
        protected override void Run(IObjectCreationExpression element, ElementProblemAnalyzerData analyzerData,
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
                        bool hasSecureStoreService = project.HasSecureStoreServiceReference();

                        if (!hasSecureStoreService && IsInvalid(element))
                        {
                            consumer.AddHighlighting(
                                new UseSecureStoreServiceHighlighting(element),
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

            IExpressionType expressionType = element.GetExpressionType();

            if (expressionType.IsResolved)
            {
                result = element.IsOneOfType(ClrTypeKeys.DBAccessTypes);
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class UseSecureStoreServiceHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.UseSecureStoreService;
        public const string Message = "Consider use Secure Store Service instead of direct db connection.";

        public IExpression Element { get; private set; }

        public UseSecureStoreServiceHighlighting(IExpression element)
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
