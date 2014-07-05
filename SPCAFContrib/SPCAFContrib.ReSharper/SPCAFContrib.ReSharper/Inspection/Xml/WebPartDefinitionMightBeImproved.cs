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

[assembly: RegisterConfigurableSeverity(WebPartDefinitionMightBeImprovedHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  WebPartDefinitionMightBeImprovedHighlighting.CheckId + ": Improve web part description.",
  "Set of checks for web part file.",
  Severity.SUGGESTION,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class WebPartDefinitionMightBeImproved : XmlTagProblemAnalyzer
    {
        private bool _catalogIconImageUrlPresent = false;
        private bool _titleIconImageUrlPresent = false;
        private bool _descriptionPresent = false;
        private bool _chromeTypePresent = false;
        private readonly string[] RestrictedDescriptions = { "My Web Part", "My Visual WebPart" };

        [Flags]
        public enum ValidationResult : uint
        {
            Valid = 0,
            MissingCatalogIconImageUrl = 1,
            MissingTitleIconImageUrl = 2,
            MissingChromeType = 4,
            MissingDescription = 8,
            RestrictedDescription = 16
        }

        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                ValidationResult validationResult = IsInvalid(element);
                if ((uint)validationResult > 0)
                {
                    WebPartDefinitionMightBeImprovedHighlighting errorHighlighting = new WebPartDefinitionMightBeImprovedHighlighting(
                        element, validationResult);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public override void Init(IXmlFile file)
        {
            base.Init(file);

            _catalogIconImageUrlPresent =
                file.GetNestedTags<IXmlTag>("webParts/webPart/data/properties/property")
                    .Any(t => t.CheckAttributeValue("name","CatalogIconImageUrl", true));
            _titleIconImageUrlPresent =
                file.GetNestedTags<IXmlTag>("webParts/webPart/data/properties/property")
                    .Any(t => t.CheckAttributeValue("name", "TitleIconImageUrl", true));
            _descriptionPresent =
                file.GetNestedTags<IXmlTag>("webParts/webPart/data/properties/property")
                    .Any(t => t.CheckAttributeValue("name", "Description", true));
            _chromeTypePresent =
                file.GetNestedTags<IXmlTag>("webParts/webPart/data/properties/property")
                    .Any(t => t.CheckAttributeValue("name", "ChromeType", true));
        }

        public ValidationResult IsInvalid(IXmlTag element)
        {
            ValidationResult result = ValidationResult.Valid;

            switch (element.Header.ContainerName)
            {
                case "properties":
                    if (!_catalogIconImageUrlPresent)
                        result |= ValidationResult.MissingCatalogIconImageUrl;
                    if (!_titleIconImageUrlPresent)
                        result |= ValidationResult.MissingTitleIconImageUrl;
                    if (!_descriptionPresent)
                        result |= ValidationResult.MissingDescription;
                    if (!_chromeTypePresent)
                        result |= ValidationResult.MissingChromeType;
                    break;
                case "property":
                    RestrictedDescriptions.ForEach(restrictedDescription =>
                    {
                        if (element.CheckAttributeValue("name", "Description", true) && element.InnerText.Trim() == restrictedDescription)
                        {
                            result |= ValidationResult.RestrictedDescription;
                        }
                    });
                    break;
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE,
        ShowToolTipInStatusBar = true)]
    public class WebPartDefinitionMightBeImprovedHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.WebPart.WebPartDefinitionMightBeImproved;

        public IXmlTag Element { get; private set; }
        public WebPartDefinitionMightBeImproved.ValidationResult ValidationResult;

        private static string GetMessage(WebPartDefinitionMightBeImproved.ValidationResult validationResult)
        {
            StringBuilder sb = new StringBuilder();

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingCatalogIconImageUrl) ==
                WebPartDefinitionMightBeImproved.ValidationResult.MissingCatalogIconImageUrl)
            {
                sb.AppendLine("Web part might have not null or empty CatalogIconImageUrl property.");
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingTitleIconImageUrl) ==
                WebPartDefinitionMightBeImproved.ValidationResult.MissingTitleIconImageUrl)
            {
                sb.AppendLine("Web part might have not null or empty TitleIconImageUrl property.");
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingChromeType) ==
                WebPartDefinitionMightBeImproved.ValidationResult.MissingChromeType)
            {
                sb.AppendLine("Web part might have not null or empty ChromeType property.");
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingDescription) ==
                WebPartDefinitionMightBeImproved.ValidationResult.MissingDescription)
            {
                sb.AppendLine("Web part should have description property.");
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.RestrictedDescription) ==
                WebPartDefinitionMightBeImproved.ValidationResult.RestrictedDescription)
            {
                sb.AppendLine("Web part should not have autogenerated description property.");
            }

            return sb.ToString().Trim();
        }

        public WebPartDefinitionMightBeImprovedHighlighting(IXmlTag element,
            WebPartDefinitionMightBeImproved.ValidationResult validationResult) :
            base(String.Format("{0}: {1}", CheckId, GetMessage(validationResult)))
        {
            ValidationResult = validationResult;
            Element = element;
        }
    }

    [QuickFix]
    public class WebPartDefinitionMightBeImprovedFix : QuickFixBase
    {
        private readonly WebPartDefinitionMightBeImprovedHighlighting _highlighting;

        public WebPartDefinitionMightBeImprovedFix([NotNull] WebPartDefinitionMightBeImprovedHighlighting highlighting)
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
                                new WebPartDefinitionMightBeImprovedFileHandler(document, xmlFile).Run(indicator);
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
                WebPartDefinitionMightBeImprovedFileHandler.Fix(_highlighting.Element, _highlighting.ValidationResult, solution);
            }

            return null;
        }

        public override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if ((_highlighting.ValidationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingCatalogIconImageUrl) ==
                WebPartDefinitionMightBeImproved.ValidationResult.MissingCatalogIconImageUrl)
                {
                    sb.Append("Add CatalogIconImageUrl property. ");
                }

                if ((_highlighting.ValidationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingTitleIconImageUrl) ==
                    WebPartDefinitionMightBeImproved.ValidationResult.MissingTitleIconImageUrl)
                {
                    sb.Append("Add TitleIconImageUrl property. ");
                }

                if ((_highlighting.ValidationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingChromeType) ==
                    WebPartDefinitionMightBeImproved.ValidationResult.MissingChromeType)
                {
                    sb.Append("Add ChromeType property. ");
                }

                if ((_highlighting.ValidationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingDescription) ==
                    WebPartDefinitionMightBeImproved.ValidationResult.MissingDescription)
                {
                    sb.Append("Add Description property. ");
                }

                return CommonHelper.NormalizeQFMessage(sb);
            }
        }
    }

    public class WebPartDefinitionMightBeImprovedFileHandler
    {
        private readonly IXmlFile _file;
        private readonly IDocument _document;
        private WebPartDefinitionMightBeImproved _rule;

        struct FileValidationResult
        {
            public WebPartDefinitionMightBeImproved.ValidationResult ValidationResult { get; set; }
            public ITreeNodePointer<IXmlTag> NodePointer { get; set; }
        }

        public WebPartDefinitionMightBeImprovedFileHandler(IDocument document, IXmlFile file)
        {
            this._file = file;
            this._document = document;
            this._rule = new WebPartDefinitionMightBeImproved();
            this._rule.Init(_file);
        }

        public void Run(IProgressIndicator pi)
        {
            List<FileValidationResult> fields = new List<FileValidationResult>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IXmlTag>(
                element =>
                {
                    WebPartDefinitionMightBeImproved.ValidationResult validationResult = _rule.IsInvalid(element);
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

        public static void Fix(IXmlTag element, WebPartDefinitionMightBeImproved.ValidationResult validationResult, ISolution solution)
        {
            XmlElementFactory elementFactory = XmlElementFactory.GetInstance(element);
            IXmlTag anchor = element.GetNestedTags<IXmlTag>("property").LastOrDefault();

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingCatalogIconImageUrl) ==
                WebPartDefinitionMightBeImproved.ValidationResult.MissingCatalogIconImageUrl)
            {
                IXmlTag tag = elementFactory.CreateTagForTag(element, "<property name=\"CatalogIconImageUrl\" type=\"string\">/_layouts/images/mscntvwl.gif</property>");
                element.AddTagAfter(tag, anchor);
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingTitleIconImageUrl) ==
            WebPartDefinitionMightBeImproved.ValidationResult.MissingTitleIconImageUrl)
            {
                IXmlTag tag = elementFactory.CreateTagForTag(element, "<property name=\"TitleIconImageUrl\" type=\"string\">/_layouts/images/mscntvwl.gif</property>");
                element.AddTagAfter(tag, anchor);
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingChromeType) ==
            WebPartDefinitionMightBeImproved.ValidationResult.MissingChromeType)
            {
                IXmlTag tag = elementFactory.CreateTagForTag(element, "<property name=\"ChromeType\" type=\"chrometype\">None</property>");
                element.AddTagAfter(tag, anchor);
            }

            if ((validationResult & WebPartDefinitionMightBeImproved.ValidationResult.MissingDescription) ==
            WebPartDefinitionMightBeImproved.ValidationResult.MissingDescription)
            {
                IXmlTag tag = elementFactory.CreateTagForTag(element, "<property name=\"Description\" type=\"string\"></property>");
                element.AddTagAfter(tag, anchor);
            }
        }
    }
}
