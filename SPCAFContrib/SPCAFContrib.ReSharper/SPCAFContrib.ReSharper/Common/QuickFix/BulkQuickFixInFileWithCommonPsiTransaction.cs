using System;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace SPCAFContrib.ReSharper.Common.QuickFix
{
    public class BulkQuickFixInFileWithCommonPsiTransaction : QuickFixBase
    {

        private readonly IProjectFile myProjectFile;
        private readonly Action<IDocument, IPsiSourceFile, IProgressIndicator> myPsiTransactionAction;
        private string _actionText;

        public BulkQuickFixInFileWithCommonPsiTransaction(IProjectFile projectFile, string actionText,
            Action<IDocument, IPsiSourceFile, IProgressIndicator> psiTransactionAction)
        {
            myProjectFile = projectFile;
            myPsiTransactionAction = psiTransactionAction;
            _actionText = actionText + " in file";
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var documentManager = solution.GetComponent<DocumentManager>();
            var document = documentManager.GetOrCreateDocument(myProjectFile);
            var psiSourceFile = myProjectFile.ToSourceFile();
            if (psiSourceFile != null)
                myPsiTransactionAction(document, psiSourceFile, progress);

            return null;
        }

        public override string Text
        {
            get { return _actionText; }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }
    }
}
