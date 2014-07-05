using System;
using System.Text;
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
using SPCAFContrib.ReSharper.Common.CodeAnalysis;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(AvoidSPObjectNameStringComparisonHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  AvoidSPObjectNameStringComparisonHighlighting.CheckId + ": Avoid SPObject.Name == <string> comparison",
  "Depending on the case, SPObject.Name string based comparison is quite unsafe and might lead to the potential issues.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IReferenceExpression) },
        HighlightingTypes = new[] { typeof(AvoidSPObjectNameStringComparisonHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class AvoidSPObjectNameStringComparison : ElementProblemAnalyzer<IReferenceExpression>
    {
        [Flags]
        public enum ValidationResult : uint
        {
            Valid = 0,
            SPPersistedObject = 1,
            SPContentType = 2,
            PageLayout = 4,
            SPListItem = 8,
            TaxonomyItem = 16,
            SPWeb = 32,
            SPPrincipal = 64
        }

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
                        ValidationResult validationResult = IsInvalid(element);
                        if ((uint)validationResult > 0)
                        {
                            consumer.AddHighlighting(
                                new AvoidSPObjectNameStringComparisonHighlighting(element, validationResult),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static ValidationResult IsInvalid(IReferenceExpression element)
        {
            ValidationResult result = ValidationResult.Valid;
            string[] propertyNames = {"Name"};

            if (element.IsResolvedAsMethodCall(ClrTypeKeys.SystemString, new[] { 
                new MethodCriteria(){ShortName = "Equals"}, 
                new MethodCriteria(){ShortName = "Compare"}, 
                new MethodCriteria(){ShortName = "CompareTo"} }))
            {
                if (element.QualifierExpression is IReferenceExpression)
                {
                    IReferenceExpression qualifierExpression = element.QualifierExpression as IReferenceExpression;
                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.SPContentType, propertyNames))
                        result |= ValidationResult.SPContentType;

                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.SPPersistedObject, propertyNames))
                        result |= ValidationResult.SPPersistedObject;

                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.PageLayout, propertyNames))
                        result |= ValidationResult.PageLayout;

                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.SPListItem, propertyNames))
                        result |= ValidationResult.SPListItem;

                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.TaxonomyItem, propertyNames))
                        result |= ValidationResult.TaxonomyItem;

                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.SPWeb, propertyNames))
                        result |= ValidationResult.SPWeb;

                    if (qualifierExpression.IsResolvedAsPropertyUsage(ClrTypeKeys.SPPrincipal, propertyNames))
                        result |= ValidationResult.SPPrincipal;
                }

                if ((uint) result == 0)
                {
                    ICSharpExpression containingExpression = element.GetContainingExpression();
                    if (containingExpression is IInvocationExpression)
                    {
                        IInvocationExpression invocationExpression = containingExpression as IInvocationExpression;
                        TreeNodeCollection<ICSharpArgument> arguments = invocationExpression.Arguments;

                        if (
                            arguments.Any(
                                argument =>
                                    argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPContentType, propertyNames)))
                            result |= ValidationResult.SPContentType;

                        if (
                            arguments.Any(
                                argument =>
                                    argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPPersistedObject, propertyNames)))
                            result |= ValidationResult.SPPersistedObject;

                        if (
                            arguments.Any(
                                argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.PageLayout, propertyNames)))
                            result |= ValidationResult.PageLayout;

                        if (
                            arguments.Any(
                                argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPListItem, propertyNames)))
                            result |= ValidationResult.SPListItem;

                        if (
                            arguments.Any(
                                argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.TaxonomyItem, propertyNames)))
                            result |= ValidationResult.TaxonomyItem;

                        if (
                            arguments.Any(
                                argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPWeb, propertyNames)))
                            result |= ValidationResult.SPWeb;

                        if (
                            arguments.Any(
                                argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPPrincipal, propertyNames)))
                            result |= ValidationResult.SPPrincipal;
                    }
                }
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class AvoidSPObjectNameStringComparisonHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.AvoidSPObjectNameStringComparison;
        public const string MESSAGE_TEMPLATE = "Avoid {0}.Name == <string> comparison. ";

        public IExpression Element { get; private set; }
        public AvoidSPObjectNameStringComparison.ValidationResult ValidationResult;

        private string GetMessage()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in EnumUtil.GetValues<AvoidSPObjectNameStringComparison.ValidationResult>())
            {
                if (item != AvoidSPObjectNameStringComparison.ValidationResult.Valid &&
                    (ValidationResult & item) == item)
                {
                    sb.Append(String.Format(MESSAGE_TEMPLATE, item));
                }
            }

            return sb.ToString().Trim();
        }

        public AvoidSPObjectNameStringComparisonHighlighting(IExpression element, AvoidSPObjectNameStringComparison.ValidationResult validationResult)
        {
            ValidationResult = validationResult;
            this.Element = element;
        }

        #region IHighlighting Members

        public string ToolTip
        {
            get
            {
                return String.Format("{0}: {1}", CheckId, GetMessage());
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
