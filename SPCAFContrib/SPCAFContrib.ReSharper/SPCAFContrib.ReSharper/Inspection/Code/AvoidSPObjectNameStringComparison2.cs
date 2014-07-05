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
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(AvoidSPObjectNameStringComparisonHighlighting.CheckId + "-2",
  null,
  Consts.CORRECTNESS_GROUP,
  AvoidSPObjectNameStringComparisonHighlighting.CheckId + ": Avoid SPObject.Name == <string> comparison",
  "Depending on the case, SPObject.Name string based comparison is quite unsafe and might lead to the potential issues.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IEqualityExpression) },
        HighlightingTypes = new[] { typeof(AvoidSPObjectNameStringComparisonHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class AvoidSPObjectNameStringComparison2 : ElementProblemAnalyzer<IEqualityExpression>
    {
        protected override void Run(IEqualityExpression element, ElementProblemAnalyzerData analyzerData,
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
                        AvoidSPObjectNameStringComparison.ValidationResult validationResult = IsInvalid(element);
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

        public static AvoidSPObjectNameStringComparison.ValidationResult IsInvalid(IEqualityExpression element)
        {
            AvoidSPObjectNameStringComparison.ValidationResult result = AvoidSPObjectNameStringComparison.ValidationResult.Valid;
            IList<ICSharpArgumentInfo> arguments = element.Arguments;
            string[] propertyNames = {"Name"};

            if (arguments.Count > 0)
            {
                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPContentType, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.SPContentType;

                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPPersistedObject, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.SPPersistedObject;

                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.PageLayout, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.PageLayout;

                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPListItem, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.SPListItem;

                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.TaxonomyItem, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.TaxonomyItem;

                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPWeb, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.SPWeb;

                if (arguments.Any(argument => argument.IsReferenceOfPropertyUsage(ClrTypeKeys.SPPrincipal, propertyNames)))
                    result |= AvoidSPObjectNameStringComparison.ValidationResult.SPPrincipal;
            }

            return result;
        }
    }
}
