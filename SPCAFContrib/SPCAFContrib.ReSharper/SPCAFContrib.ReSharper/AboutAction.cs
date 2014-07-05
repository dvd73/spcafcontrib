using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace SPCAFContrib.ReSharper
{
    [ActionHandler("SPCAFContrib.ReSharper.About")]
    public class AboutAction : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show(
              "Essential tool to ensure SharePoint code quality.\r\nVisit our site http://spcafcontrib.codeplex.com",
              "About SPCAF Contrib ReSharper Plugin",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}