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
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Consts;
using SPCAFContrib.ReSharper.Inspection.Code;

[assembly: RegisterConfigurableSeverity(MagicStringShouldNotBeUsedHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  MagicStringShouldNotBeUsedHighlighting.CheckId + ": Hardcoded magic string detected",
  "Do not use hardcoded urls, pathes, emails and account names in code.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(ILiteralExpression) }, HighlightingTypes = new[] { typeof(MagicStringShouldNotBeUsedHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class MagicStringShouldNotBeUsed : ElementProblemAnalyzer<ILiteralExpression>
    {
        [Flags]
        public enum ValidationResult : uint
        {
            Valid = 0,
            Url = 1,
            Path = 2,
            EMail = 4,
            AccountName = 8
        }

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
                        ValidationResult validationResult = IsInvalid(element);
                        if ((uint)validationResult > 0)
                        {
                            consumer.AddHighlighting(
                                new MagicStringShouldNotBeUsedHighlighting(element, validationResult),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static ValidationResult IsInvalid(ILiteralExpression element)
        {
            ValidationResult result = ValidationResult.Valid;

            IAttribute r = element.GetContainingNode<IAttribute>();

            if (r == null && element.ConstantValue.IsString())
            {
                string literal = element.ConstantValue.Value.ToString();
                switch (MagicStringsHelper.Match(literal))
                {
                    case "Uri":
                        result = ValidationResult.Url;
                        break;
                    case "Email":
                        result = ValidationResult.EMail;
                        break;
                    case "Path":
                        result = ValidationResult.Path;
                        break;
                    case "AccountName":
                        result = ValidationResult.AccountName;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }


    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.WARNING, ShowToolTipInStatusBar = true)]
    public class MagicStringShouldNotBeUsedHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.MagicStringShouldNotBeUsed;
        public const string MESSAGE_TEMPLATE = "Hardcoded {0} is detected.";

        public IExpression Element { get; private set; }
        public MagicStringShouldNotBeUsed.ValidationResult ValidationResult;

        private string GetMessage()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in EnumUtil.GetValues<MagicStringShouldNotBeUsed.ValidationResult>())
            {
                if (item != MagicStringShouldNotBeUsed.ValidationResult.Valid &&
                    (ValidationResult & item) == item)
                {
                    sb.Append(String.Format(MESSAGE_TEMPLATE, item));
                }
            }

            return sb.ToString().Trim();
        }

        public MagicStringShouldNotBeUsedHighlighting(IExpression element, MagicStringShouldNotBeUsed.ValidationResult validationResult)
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
