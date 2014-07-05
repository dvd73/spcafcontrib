using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Html;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using SPCAFContrib.Entities.Consts;
using SPCAFContrib.ReSharper.Common;
using SPCAFContrib.ReSharper.Common.Attributes;
using SPCAFContrib.ReSharper.Common.Extensions;
using SPCAFContrib.ReSharper.Inspection.Code;
using SPCAFContrib.ReSharper.Consts;

[assembly: RegisterConfigurableSeverity(AvoidStaicSPObjectsInFieldsHighlighting.CheckId,
  null,
  Consts.BEST_PRACTICE_GROUP,
  AvoidStaicSPObjectsInFieldsHighlighting.CheckId + ": " + AvoidStaicSPObjectsInFieldsHighlighting.Message,
  "Having static SP-Objects as a fields are quite dangerous.",
  Severity.WARNING,
  false, Internal = false)]

namespace SPCAFContrib.ReSharper.Inspection.Code
{
    [ElementProblemAnalyzer(new[] { typeof(IMultipleFieldDeclaration) }, HighlightingTypes = new[] { typeof(AvoidStaicSPObjectsInFieldsHighlighting) })]
    [Applicability(
        IDEProjectType.SP2010FarmSolution |
        IDEProjectType.SPSandbox |
        IDEProjectType.SP2013FarmSolution |
        IDEProjectType.SPServerAPIReferenced)]
    public class AvoidStaicSPObjectsInFields : ElementProblemAnalyzer<IMultipleFieldDeclaration>
    {
        protected override void Run(IMultipleFieldDeclaration element, ElementProblemAnalyzerData analyzerData,
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
                                new AvoidStaicSPObjectsInFieldsHighlighting(element),
                                element.GetDocumentRange(),
                                element.GetContainingFile());
                        }
                    }
                }
            }
        }

        public static bool IsInvalid(IMultipleFieldDeclaration element)
        {
            return element.ModifiersList != null && element.ModifiersList.IsAny(modifier => modifier.HasModifier(CSharpTokenType.STATIC_KEYWORD)) &&
                   element.Declarators.Any(CheckDeclarator);
        }

        private static bool CheckDeclarator(IMultipleDeclarationMember declarator)
        {
            bool result = false;
            IClrTypeName[] typeNames = {ClrTypeKeys.SPWeb, ClrTypeKeys.SPSite, ClrTypeKeys.SPFolder, ClrTypeKeys.SPListItem, ClrTypeKeys.SPFile};

            if (declarator is IFieldDeclaration)
            {
                ITypeElement containingType = (declarator as IFieldDeclaration).DeclaredElement.Type().GetTypeElement<ITypeElement>();
                if (containingType != null)
                {
                    result = typeNames.Any(
                                    typeName =>
                                        containingType.GetClrName().Equals(typeName));
                }
            }

            return result;
        }
    }

    [ConfigurableSeverityHighlighting(CheckId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = true)]
    public class AvoidStaicSPObjectsInFieldsHighlighting : IHighlighting
    {
        public const string CheckId = CheckIDs.Rules.Assembly.AvoidStaticSPObjectsInFields;
        public const string Message = "Avoid using static SharePoint type object as field";

        public IMultipleFieldDeclaration Element { get; private set; }

        public AvoidStaicSPObjectsInFieldsHighlighting(IMultipleFieldDeclaration element)
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
