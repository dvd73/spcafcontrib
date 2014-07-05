using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Xml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Bulk;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Intentions.QuickFixes.Bulk;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Pointers;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Parsing;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Common.QuickFix;
using SPCAFContrib.ReSharper.Common.XmlAnalysis;
using SPCAFContrib.ReSharper.Inspection.Xml;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableSeverity(DeployFieldsCorrectlyHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DeployFieldsCorrectlyHighlighting.CheckId + ": Deploy fields correctly.",
  "Set of principal checks for field provision.",
  Severity.ERROR,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DeployFieldsCorrectly : XmlTagProblemAnalyzer
    {
        [Flags]
        public enum ValidationResult : uint
        {
            Valid = 0,
            Version = 1,
            ShowField = 2,
            WebId = 4,
            ListWebRelativeListUrl = 8,
            ListGuid = 16
        }

        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                ValidationResult validationResult = IsInvalid(element);
                if ((uint) validationResult > 0)
                {
                    DeployFieldsCorrectlyHighlighting errorHighlighting = new DeployFieldsCorrectlyHighlighting(
                        element, validationResult);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public static ValidationResult IsInvalid(IXmlTag element)
        {
            ValidationResult result = ValidationResult.Valid;

            if (element.IsFieldDefinition())
            {
                if (element.AttributeExists("Version"))
                    result |= ValidationResult.Version;

                // check lookup field declaration
                if (element.CheckAttributeValue("Type", "lookup"))
                {
                    if (!element.CheckAttributeValue("ShowField", "title", true))
                        result |= ValidationResult.ShowField;

                    if (element.AttributeExists("WebId") && !element.CheckAttributeValue("WebId", "~sitecollection"))
                        result |= ValidationResult.WebId;

                    if (!element.AttributeExists("List"))
                    {
                        result |= ValidationResult.ListWebRelativeListUrl;
                    }
                    else
                    {
                        if (element.AttributeValueIsGuid("List"))
                        {
                            result |= ValidationResult.ListGuid;
                        }
                    }
                }
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE,
        ShowToolTipInStatusBar = true)]
    public class DeployFieldsCorrectlyHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.FieldTemplate.DeployFieldsCorrectly;

        public IXmlTag Element { get; private set; }
        public DeployFieldsCorrectly.ValidationResult ValidationResult;

        private static string GetMessage(DeployFieldsCorrectly.ValidationResult validationResult)
        {
            StringBuilder sb = new StringBuilder();

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.Version) ==
                DeployFieldsCorrectly.ValidationResult.Version)
            {
                sb.Append("Remove Version attribute from field. ");
            }

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.ShowField) ==
                DeployFieldsCorrectly.ValidationResult.ShowField)
            {
                sb.Append("Add ShowField=\"Title\" attribute. ");
            }

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.WebId) ==
                DeployFieldsCorrectly.ValidationResult.WebId)
            {
                sb.Append("Set WebId=\"~sitecollection\" or remove WebId attribute from field. ");
            }

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.ListWebRelativeListUrl) ==
                DeployFieldsCorrectly.ValidationResult.ListWebRelativeListUrl)
            {
                sb.Append("Add List=\"{WebRelativeListUrl}\" attribute. ");
            }

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.ListGuid) ==
                DeployFieldsCorrectly.ValidationResult.ListGuid)
            {
                sb.Append("Change List attribute from GUID to ListUrl. ");
            }

            return sb.ToString().Trim();
        }

        public DeployFieldsCorrectlyHighlighting(IXmlTag element,
            DeployFieldsCorrectly.ValidationResult validationResult) :
                base(String.Format("{0}: {1}", CheckId, GetMessage(validationResult)))
        {
            ValidationResult = validationResult;
            Element = element;
        }
    }

    [QuickFix]
    public class DeployFieldsCorrectlyFix : QuickFixBase
    {
        private readonly DeployFieldsCorrectlyHighlighting _highlighting;

        public DeployFieldsCorrectlyFix([NotNull] DeployFieldsCorrectlyHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public override IEnumerable<IntentionAction> CreateBulbItems()
        {
            if (!String.IsNullOrEmpty(Text))
            {
                ISolution solution = _highlighting.Element.GetSolution();
                IPsiFiles psiFiles = solution.GetComponent<IPsiFiles>();
                IProjectFile projectFile = _highlighting.Element.GetSourceFile().ToProjectFile();

                Action<IDocument, IPsiSourceFile, IProgressIndicator> processFileAction =
                    (document, psiSourceFile, indicator) =>
                    {
                        if (!psiSourceFile.HasExcluded(psiSourceFile.GetSettingsStore()))
                        {
                            IEnumerable<IXmlFile> xmlFiles =
                                psiFiles.GetPsiFiles<XmlLanguage>(psiSourceFile).OfType<IXmlFile>();
                            foreach (IXmlFile xmlFile in xmlFiles)
                            {
                                new DeployFieldsCorrectlyFileHandler(document, xmlFile).Run(indicator);
                            }
                        }
                    };

                var acceptProjectFilePredicate =
                    BulkItentionsBuilderEx.CreateAcceptFilePredicateByPsiLanaguage<XmlLanguage>(solution);
                var inFileFix = new BulkQuickFixInFileWithCommonPsiTransaction(projectFile, Text, processFileAction);
                var builder = new BulkQuickFixWithCommonTransactionBuilder(
                    this, inFileFix, solution, Text, processFileAction, acceptProjectFilePredicate);

                return builder.CreateBulkActions(projectFile,
                    IntentionsAnchors.QuickFixesAnchor, IntentionsAnchors.QuickFixesAnchorPosition);
            }
            else
                return EmptyList<IntentionAction>.InstanceList;
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return _highlighting.IsValid();
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            if (_highlighting.Element.IsValid())
            {
                DeployFieldsCorrectlyFileHandler.Fix(_highlighting.Element, _highlighting.ValidationResult, solution);
            }

            return null;
        }

        public override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if ((_highlighting.ValidationResult & DeployFieldsCorrectly.ValidationResult.Version) ==
                DeployFieldsCorrectly.ValidationResult.Version)
                {
                    sb.AppendLine("Remove Version attribute. ");
                }

                if ((_highlighting.ValidationResult & DeployFieldsCorrectly.ValidationResult.ShowField) ==
                    DeployFieldsCorrectly.ValidationResult.ShowField)
                {
                    sb.AppendLine("Ensure ShowField=\"Title\" attribute. ");
                }

                if ((_highlighting.ValidationResult & DeployFieldsCorrectly.ValidationResult.ListWebRelativeListUrl) ==
                    DeployFieldsCorrectly.ValidationResult.ListWebRelativeListUrl)
                {
                    sb.AppendLine("Add List=\"{WebRelativeListUrl}\" attribute. ");
                }

                return CommonHelper.NormalizeQFMessage(sb);
            }
        }
    }

    public class DeployFieldsCorrectlyFileHandler
    {
        private readonly IXmlFile _file;
        private readonly IDocument _document;

        struct FileValidationResult
        {
            public DeployFieldsCorrectly.ValidationResult ValidationResult { get; set; }
            public ITreeNodePointer<IXmlTag> NodePointer { get; set; }
        }

        public DeployFieldsCorrectlyFileHandler(IDocument document, IXmlFile file)
        {
            this._file = file;
            this._document = document;
        }

        public void Run(IProgressIndicator pi)
        {
            List<FileValidationResult> fields = new List<FileValidationResult>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IXmlTag>(
                element =>
                {
                    DeployFieldsCorrectly.ValidationResult validationResult = DeployFieldsCorrectly.IsInvalid(element);
                    if ((uint)validationResult > 0)
                        fields.Add(
                            new FileValidationResult()
                            {
                                NodePointer = element.CreateTreeElementPointer(),
                                ValidationResult = validationResult
                            });
                }));

            pi.Start(fields.Count);
            foreach (FileValidationResult fileValidationResult in fields)
            {
                IXmlTag node = fileValidationResult.NodePointer.GetTreeNode();
                if (node != null)
                {
                    Fix(node, fileValidationResult.ValidationResult, _file.GetSolution());
                }
                pi.Advance(1.0);
            }
        }

        public static void Fix(IXmlTag element, DeployFieldsCorrectly.ValidationResult validationResult, ISolution solution)
        {
            XmlElementFactory elementFactory = XmlElementFactory.GetInstance(element);
            if ((validationResult & DeployFieldsCorrectly.ValidationResult.Version) ==
                    DeployFieldsCorrectly.ValidationResult.Version)
            {
                element.RemoveAttribute(element.GetAttribute("Version"));
            }

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.ShowField) ==
                DeployFieldsCorrectly.ValidationResult.ShowField)
            {
                element.EnsureAttribute("ShowField", "Title");
            }

            if ((validationResult & DeployFieldsCorrectly.ValidationResult.ListWebRelativeListUrl) ==
            DeployFieldsCorrectly.ValidationResult.ListWebRelativeListUrl)
            {
                element.EnsureAttribute("List", "{WebRelativeListUrl}");
            }
        }
    }
}
