using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(SPMonitoredScopeShouldBeUsedHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  SPMonitoredScopeShouldBeUsedHighlighting.CheckId + ": " + SPMonitoredScopeShouldBeUsedHighlighting.Message,
  "Some recommended practices regarding SPMonitoredScope class utilization.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IClassDeclaration) }, HighlightingTypes = new[] { typeof(SPMonitoredScopeShouldBeUsedHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class SPMonitoredScopeShouldBeUsed : ElementProblemAnalyzer<IClassDeclaration>
    {
        protected override void Run(IClassDeclaration element, ElementProblemAnalyzerData analyzerData,
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
                                new SPMonitoredScopeShouldBeUsedHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IClassDeclaration element)
        {
            bool result = false;

            if (element.DeclaredElement != null)
            {
                IEnumerable<IDeclaredType> parentTypes = element.DeclaredElement.GetAllSuperClasses();

                if (!parentTypes.Any(parentType => parentType.GetClrName().Equals(ClrTypeKeys.Page) || parentType.GetClrName().Equals(ClrTypeKeys.MasterPage)))
                {
                    result = ClrTypeKeys.AllowedWebControls.Any(typeName => parentTypes.Any(
                        parentType => parentType.GetClrName().Equals(typeName)));


                    if (result && (from node in element.EnumerateSubTree().OfType<IObjectCreationExpression>()
                        select node
                        into objectCreationExpression
                        let expressionType = objectCreationExpression.GetExpressionType()
                        where
                            expressionType.IsResolved &&
                            objectCreationExpression.IsOneOfType(new[] {ClrTypeKeys.SPMonitoredScope})
                        select objectCreationExpression).Any())
                    {
                        result = false;
                    }
                }
            }
            

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class SPMonitoredScopeShouldBeUsedHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.SPMonitoredScopeShouldBeUsed;
        public const string Message = "SPMonitoredScope should be used";

        public IClassDeclaration Element { get; private set; }

        public SPMonitoredScopeShouldBeUsedHighlighting(IClassDeclaration element)
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
