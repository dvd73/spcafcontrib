using System;
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.UI.Application;
using JetBrains.UI.BulbMenu;
using JetBrains.UI.Resources;
using SPCAFContrib.ReSharper.Common.Shell;

namespace SPCAFContrib.ReSharper.Common.HelpLink
{
    [CustomHighlightingActionProvider(typeof(KnownProjectFileType))]
    public class CodeInspectionHelpLinkProvider : ICustomHighlightingActionProvider
    {
        private readonly ICodeInspectionHelpLinkDataProvider myDataProvider;
        private readonly UIApplication myUiApplication;

        public CodeInspectionHelpLinkProvider(ICodeInspectionHelpLinkDataProvider dataProvider, UIApplication uiApplication)
        {
            this.myDataProvider = dataProvider;
            this.myUiApplication = uiApplication;
        }

        /// <summary>
        /// Gets the actions for open help page for SPCAF Contrib rules.
        /// </summary>
        /// <param name="highlighting"></param>
        /// <param name="highlightingRange"></param>
        /// <param name="sourceFile"></param>
        /// <param name="inspectionTitle"></param>
        /// <returns></returns>
        public IEnumerable<IntentionAction> GetActions(IHighlighting highlighting, DocumentRange highlightingRange,
            IPsiSourceFile sourceFile, string inspectionTitle)
        {
            string id = HighlightingExtensions.GetConfigurableSeverityId(highlighting);
            string url;
            if (this.myDataProvider.TryGetValue(id ?? String.Empty, out url))
            {
                CodeInspectionDocLinkAction action = new CodeInspectionDocLinkAction(this.myUiApplication, url);
                IAnchor anchor = ConfigureHighlightingAnchors.GetConfigureAnchor(ConfigureHighlightingAnchors.CodeInspectionWiki, highlighting);
                yield return
                    new IntentionAction(action, action.Text, CommonThemedIcons.Question.Id, anchor);
            }
        }

        private sealed class CodeInspectionDocLinkAction : IBulbAction
        {
            private readonly UIApplication myUiApplication;
            private readonly string myUrl;

            public string Text
            {
                get { return "Why is SPCAF Contrib suggesting this?"; }
            }

            public CodeInspectionDocLinkAction(UIApplication uiApplication, string url)
            {
                this.myUiApplication = uiApplication;
                this.myUrl = url;
            }

            public void Execute(ISolution solution, ITextControl textControl)
            {
                IUIApplicationSimpleEx.OpenUri((IUIApplicationSimple) this.myUiApplication, this.myUrl);
            }
        }
    }
}
