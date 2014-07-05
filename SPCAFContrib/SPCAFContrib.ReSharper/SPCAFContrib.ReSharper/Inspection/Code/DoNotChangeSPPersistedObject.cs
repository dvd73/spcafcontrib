using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.CodeAnalysis;
using SPCAFContrib.ReSharper.Common.DataCache;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(DoNotChangeSPPersistedObjectHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DoNotChangeSPPersistedObjectHighlighting.CheckId + ": " + DoNotChangeSPPersistedObjectHighlighting.Message,
  "SharePoint 2010+ has security feature to all objects inheriting from SPPersistedObject. This feature explicitly disallows modification of SPPersistedObject objects from content web applications.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IReferenceExpression) },
        HighlightingTypes = new[] { typeof(DoNotChangeSPPersistedObjectHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class DoNotChangeSPPersistedObject : ElementProblemAnalyzer<IReferenceExpression>
    {
        protected override void Run(IReferenceExpression element, ElementProblemAnalyzerData analyzerData,
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
                                new DoNotChangeSPPersistedObjectHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IReferenceExpression element)
        {
            bool result = false;
            IExpressionType expressionType = element.GetExpressionType();

            if (expressionType.IsResolved &&
                element.IsResolvedAsMethodCall(ClrTypeKeys.SPPersistedObject, new[] { new MethodCriteria() { ShortName = "Update" } }))
            {
                var project = element.GetProject();
                string assemblyFullName = project.GetOutputAssemblyFullName();
                var containingTypeDeclaration = element.GetContainingTypeDeclaration().CLRName;
                string containerReadableName = element.ContainerReadableName();

                result =
                    !FeatureCache.GetInstance(project.GetSolution())
                        .GetReceivers(SPFeatureScope.WebApplication)
                        .Any(
                            receiver =>
                                String.Equals(receiver.ReceiverAssembly, assemblyFullName,
                                    StringComparison.OrdinalIgnoreCase) &&
                                (String.Equals(receiver.ReceiverClass, containingTypeDeclaration,
                                    StringComparison.OrdinalIgnoreCase) ||
                                 receiver.ReceiverClassReferences.Any(
                                     reference => reference.Equals(containerReadableName)))
                        );
            }

            return result;
        }

    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DoNotChangeSPPersistedObjectHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.DoNotChangeSPPersistedObject;
        public const string Message = "Do not change SPPersistedObject in the content web application";

        public IExpression Element { get; private set; }

        public DoNotChangeSPPersistedObjectHighlighting(IExpression element)
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
