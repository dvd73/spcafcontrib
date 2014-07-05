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

[assembly: RegisterConfigurableSeverity(DeployTaxonomyFieldsCorrectlyHighlighting.CheckId,
  null,
  Consts.CORRECTNESS_GROUP,
  DeployTaxonomyFieldsCorrectlyHighlighting.CheckId + ": Deploy taxonomy fields correctly.",
  "Set of principal checks for taxonomy field provision.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Xml
{
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution)]
    public class DeployTaxonomyFieldsCorrectly : XmlTagProblemAnalyzer
    {
        [Flags]
        public enum ValidationResult : uint
        {
            Valid       = 0,
            ShowField   = 1,
            Mult        = 2,
            TextField   = 4
        }

        private IXmlTag _invalidProperty;

        public override void Init(IXmlFile file)
        {
            _invalidProperty = null;
            base.Init(file);
        }

        public override void Run(IXmlTag element, IHighlightingConsumer consumer)
        {
            if (element.GetProject().IsApplicableFor(this))
            {
                ValidationResult validationResult = IsInvalid(element);
                if ((uint)validationResult > 0)
                {
                    DeployTaxonomyFieldsCorrectlyHighlighting errorHighlighting = new DeployTaxonomyFieldsCorrectlyHighlighting(element, validationResult);
                    consumer.ConsumeHighlighting(element.Header.GetDocumentRange(), errorHighlighting);
                }
            }
        }

        public ValidationResult IsInvalid(IXmlTag element)
        {
            ValidationResult result = ValidationResult.Valid;

            if (element.IsFieldDefinition())
            {
                if (element.CheckAttributeValue("Type", "TaxonomyFieldType"))
                {
                    if (element.AttributeExists("ShowField") && !element.CheckAttributeValue("ShowField", "Term$Resources:core,Language;", true))
                        result |= ValidationResult.ShowField;

                    if (element.CheckAttributeValue("Type", "TaxonomyFieldTypeMulti", true) &&
                        (!element.AttributeExists("Mult") || element.CheckAttributeValue("Mult", "false", true)))
                        result |= ValidationResult.Mult;
                    
                    IXmlTag tagCustomization = element.InnerTags.SingleOrDefault(_ => _.Header.Name.XmlName == "Customization");

                    if (tagCustomization != null)
                    {
                        IXmlTag tagArrayOfProperty =
                            tagCustomization.InnerTags.SingleOrDefault(_ => _.Header.Name.XmlName == "ArrayOfProperty");

                        if (tagArrayOfProperty != null)
                        {
                            IXmlTag invalidProperty = tagArrayOfProperty.InnerTags
                                .SingleOrDefault(
                                    _ =>
                                        _.Header.Name.XmlName == "Property" &&
                                        _.InnerTags.Any(
                                            __ => __.Header.Name.XmlName == "Name" && __.InnerValue == "TextField"));
                                
                            if (invalidProperty != null)
                            {
                                _invalidProperty = invalidProperty;
                            }
                        }
                    }
                }
            }
            else if (element.Header.ContainerName == "Property")
            {
                
                if (element.Equals(_invalidProperty))
                    result |= ValidationResult.TextField;
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class DeployTaxonomyFieldsCorrectlyHighlighting : XmlErrorHighlighting
    {
        public const string CheckId = CheckIDs.Rules.FieldTemplate.DeployTaxonomyFieldsCorrectly;
        public IXmlTag Element { get; private set; }
        public DeployTaxonomyFieldsCorrectly.ValidationResult ValidationResult { get; private set; }

        private static string GetMessage(DeployTaxonomyFieldsCorrectly.ValidationResult validationResult)
        {
            StringBuilder sb = new StringBuilder();

            if ((validationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.ShowField) ==
                DeployTaxonomyFieldsCorrectly.ValidationResult.ShowField)
            {
                sb.Append("Remove ShowField attribute. ");
            }

            if ((validationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.Mult) ==
                DeployTaxonomyFieldsCorrectly.ValidationResult.Mult)
            {
                sb.Append("Add Mult=\"TRUE\" attribute. ");
            }

            if ((validationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.TextField) ==
                DeployTaxonomyFieldsCorrectly.ValidationResult.TextField)
            {
                sb.Append("Do not specify TextField property. ");
            }

            return sb.ToString().Trim();
        }

        public DeployTaxonomyFieldsCorrectlyHighlighting(IXmlTag element, DeployTaxonomyFieldsCorrectly.ValidationResult validationResult) :
            base(String.Format("{0}: {1}", CheckId, GetMessage(validationResult)))
        {
            ValidationResult = validationResult;
            Element = element;
        }
    }

    [QuickFix]
    public class DeployTaxonomyFieldsCorrectlyFix : QuickFixBase
    {
        private readonly DeployTaxonomyFieldsCorrectlyHighlighting _highlighting;

        public DeployTaxonomyFieldsCorrectlyFix([NotNull] DeployTaxonomyFieldsCorrectlyHighlighting highlighting)
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
                                new DeployTaxonomyFieldsCorrectlyFileHandler(document, xmlFile).Run(indicator);
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
                DeployTaxonomyFieldsCorrectlyFileHandler.Fix(_highlighting.Element, _highlighting.ValidationResult, solution);
            }

            return null;
        }

        public override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if ((_highlighting.ValidationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.ShowField) ==
                    DeployTaxonomyFieldsCorrectly.ValidationResult.ShowField)
                {
                    sb.AppendLine("Remove ShowField attribute. ");
                }

                if ((_highlighting.ValidationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.Mult) ==
                    DeployTaxonomyFieldsCorrectly.ValidationResult.Mult)
                {
                    sb.AppendLine("Add Mult=\"TRUE\" attribute. ");
                }

                if ((_highlighting.ValidationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.TextField) ==
                DeployTaxonomyFieldsCorrectly.ValidationResult.TextField)
                {
                    sb.AppendLine("Remove TextField property. ");
                }

                return CommonHelper.NormalizeQFMessage(sb); 
            }
        }
    }

    public class DeployTaxonomyFieldsCorrectlyFileHandler
    {
        private readonly IXmlFile _file;
        private readonly IDocument _document;
        private DeployTaxonomyFieldsCorrectly _rule;

        struct FileValidationResult
        {
            public DeployTaxonomyFieldsCorrectly.ValidationResult ValidationResult { get; set; }
            public ITreeNodePointer<IXmlTag> NodePointer { get; set; }
        }

        public DeployTaxonomyFieldsCorrectlyFileHandler(IDocument document, IXmlFile file)
        {
            this._file = file;
            this._document = document;
            this._rule = new DeployTaxonomyFieldsCorrectly();
            this._rule.Init(file);
        }

        public void Run(IProgressIndicator pi)
        {
            List<FileValidationResult> fields = new List<FileValidationResult>();

            _file.ProcessThisAndDescendants(new RecursiveElementProcessor<IXmlTag>(
                element =>
                {
                    DeployTaxonomyFieldsCorrectly.ValidationResult validationResult = _rule.IsInvalid(element);
                    if ((uint) validationResult > 0)
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

        public static void Fix(IXmlTag element, DeployTaxonomyFieldsCorrectly.ValidationResult validationResult, ISolution solution)
        {
            if ((validationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.ShowField) ==
                    DeployTaxonomyFieldsCorrectly.ValidationResult.ShowField)
            {
                element.RemoveAttribute(element.GetAttribute("ShowField"));
            }

            if ((validationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.Mult) ==
                DeployTaxonomyFieldsCorrectly.ValidationResult.Mult)
            {
                element.EnsureAttribute("Mult", "TRUE");
            }

            if ((validationResult & DeployTaxonomyFieldsCorrectly.ValidationResult.TextField) ==
            DeployTaxonomyFieldsCorrectly.ValidationResult.TextField)
            {
                element.Remove();
            }
        }
    }
}
