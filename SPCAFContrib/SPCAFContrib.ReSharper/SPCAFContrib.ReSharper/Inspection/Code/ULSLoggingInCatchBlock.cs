using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Inspection.Code;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableSeverity(ULSLoggingInCatchBlockHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  ULSLoggingInCatchBlockHighlighting.CheckId + ": " + ULSLoggingInCatchBlockHighlighting.Message,
  "Catch block should include ULS logging output or re-throw.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(ICatchClause) }, HighlightingTypes = new[] { typeof(ULSLoggingInCatchBlockHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class ULSLoggingInCatchBlock : ElementProblemAnalyzer<ICatchClause>
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
                        var psiModule = sourceFile.GetPsiModule().GetContextFromModule();
                        var services = sourceFile.GetSolution().GetPsiServices();
                        var customLoggers = services.Symbols.GetSymbolScope(LibrarySymbolScope.FULL, false,
                          psiModule).GetPossibleInheritors("SPDiagnosticsServiceBase", psiModule);

                        if (IsInvalid(element, customLoggers))
                        {
                            consumer.AddHighlighting(
                                new ULSLoggingInCatchBlockHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        // it could be IGeneralCatchClause or ISpecificCatchClause(with exception type)
        public bool IsInvalid(ICatchClause element, 
            ICollection<ITypeElement> customLoggers)
        {
            bool result = false;
            var services = element.GetSolution().GetPsiServices();

            foreach (ICSharpStatement statement in element.Body.Statements)
            {
                if (statement is IThrowStatement)
                {
                    result = true;
                    break;
                }
                else if (statement is IExpressionStatement)
                {
                    result = (statement as IExpressionStatement).CheckExpression(
                        expressionStatement =>
                        {
                            return expressionStatement.IsOneOfTheTypes(customLoggers.Select(
                                customLogger => customLogger.GetClrName())
                                .Union(new[] {ClrTypeKeys.SPDiagnosticsServiceBase}));

                        },
                        method =>
                        {
                            return CheckExternalMethodCall(method, customLoggers, services);
                        }, 20);
                    
                    if (result) break;
                }
            }

            return !result;
        }

        private bool CheckExternalMethodCall(IMethod method, ICollection<ITypeElement> customLoggers, IPsiServices services)
        {
            bool result = false;
            var module = method.Module;

            result |= customLoggers.Where(
                customLogger =>
                    !customLogger.GetClrName().FullName.Contains("Microsoft.SharePoint")).Any(
                        customLogger =>
                            services.Symbols.GetSymbolScope(LibrarySymbolScope.FULL, false,
                                module.GetContextFromModule())
                                .GetElementsByQualifiedName(customLogger.GetClrName().FullName).Any());

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class ULSLoggingInCatchBlockHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.ULSLoggingInCatchBlock;
        public const string Message = "Not logged exception";

        public ICatchClause Element { get; private set; }

        public ULSLoggingInCatchBlockHighlighting(ICatchClause element)
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
