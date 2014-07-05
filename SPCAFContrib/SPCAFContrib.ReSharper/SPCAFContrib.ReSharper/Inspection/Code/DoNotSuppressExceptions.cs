using System;
using System.Linq;
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
using SPCAFContrib.ReSharper.Inspection.Code;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableSeverity(DoNotSuppressExceptionsHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DoNotSuppressExceptionsHighlighting.CheckId + ": " + DoNotSuppressExceptionsHighlighting.Message,
  "Rethrow exception in catch(Exception) block using throw or catch specific exception.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(ICatchClause) }, HighlightingTypes = new[] { typeof(DoNotSuppressExceptionsHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class DoNotSuppressExceptions : ElementProblemAnalyzer<ICatchClause>
    {
        protected override void Run(ICatchClause element, ElementProblemAnalyzerData analyzerData, IHighlightingConsumer consumer)
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
                                new DoNotSuppressExceptionsHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public bool IsInvalid(ICatchClause element)
        {
            return ((element is IGeneralCatchClause ||
                     element.ExceptionType.GetClrName().Equals(ClrTypeKeys.SystemException)) &&
                    !element.Body.Statements.OfType<IThrowStatement>().Any());
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotSuppressExceptionsHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.DoNotSuppressExceptions;
        public const string Message = "Do not suppress general exception";

        public ICatchClause Element { get; private set; }

        public DoNotSuppressExceptionsHighlighting(ICatchClause element)
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
