using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using JetBrains.Application;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store;
using JetBrains.CommonControls;
using JetBrains.CommonControls.Validation;
using JetBrains.DataFlow;
using JetBrains.IDE.Resources;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Environment.Options.Inspections;
using JetBrains.ReSharper.Psi.GeneratedCode;
using JetBrains.ReSharper.Psi.JavaScript.Resources;
using JetBrains.ReSharper.Psi.Xml.Resources;
using JetBrains.Threading;
using JetBrains.UI.Application;
using JetBrains.UI.CommonControls.Fonts;
using JetBrains.UI.Controls;
using JetBrains.UI.Icons;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;
using JetBrains.UI.Resources;
using SPCAFContrib.ReSharper.Options;

namespace SPCAFContrib.ReSharper.UI
{
    /// <summary>
    /// Copied from CodeInspectionGeneratedPage
    /// </summary>
    [OptionsPage(PID, "SPCAF Contrib", typeof(OptionsThemedIcons.Options), ParentId = CodeInspectionPage.PID)]
    public class SPCAFContribSettingsOptionPage : AStackPanelOptionsPage
    {
        private const string PID = "CodeInspection.SPCAFContribSettingsOptionPageId";
        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _settings;
        private IWindowsHookManager _windowsHookManager;
        private FormValidators _formValidators;
        private ISolution _solution;
        private IMainWindow _mainWindow;

        private StringCollectionEdit JSFileMasks;
        private StringCollectionEdit XmlFileMasks;
        private StringCollectionEdit OtherFileMasks;

        public SPCAFContribSettingsOptionPage(IUIApplication environment, OptionsSettingsSmartContext smartContext,
            Lifetime lifetime, IShellLocks shellLocks, IWindowsHookManager windowsHookManager,
            FormValidators formValidators, FontsManager fontsManager, IThemedIconManager uiIconsComponent,
            ISolution solution = null, IMainWindow mainWindow = null)
            : base(lifetime, environment, PID)
        {
            _lifetime = lifetime;
            _settings = smartContext;
            _windowsHookManager = windowsHookManager;
            _formValidators = formValidators;
            _solution = solution;
            _mainWindow = mainWindow;
            
            InitControls();

            shellLocks.QueueRecurring(lifetime, "Force settings merge", TimeSpan.FromMilliseconds(300.0),
                () => OnOk());
        }

        private void InitControls()
        {
            using (new LayoutSuspender(this))
            {
                TableLayoutPanel tablePanel = new TableLayoutPanel();
                tablePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                tablePanel.Margin = new Padding(0);
                tablePanel.Padding = new Padding(0);
                tablePanel.ColumnStyles.Insert(0, new ColumnStyle(SizeType.Percent, 100f));
                tablePanel.RowCount = 2;
                tablePanel.RowStyles.Insert(0, new RowStyle(SizeType.Percent, 50f));
                tablePanel.RowStyles.Insert(1, new RowStyle(SizeType.Percent, 50f));
                tablePanel.Size = ClientSize;
                Controls.Add(tablePanel);

                GroupingEvent ReSizeEvent = Environment.Threading.GroupingEvents[Rgc.Invariant].CreateEvent(_lifetime, "SPCAFContribSettingsOptionPage.SizeChanged", TimeSpan.FromMilliseconds(300.0), () =>
                {
                    Size size = new Size(ClientSize.Width - 10, ClientSize.Height - 10);
                    if (size.Equals(tablePanel.Size))
                        return;
                    using (new LayoutSuspender(this))
                        tablePanel.Size = size;
                });
                EventHandler ReSizeHandler = (sender, args) => ReSizeEvent.FireIncoming();
                _lifetime.AddBracket(() => SizeChanged += ReSizeHandler, () => SizeChanged -= ReSizeHandler);

                JSFileMasks = new StringCollectionEdit(Environment, "Ignored JavaScript and TypeScript files:",
                    PsiJavaScriptThemedIcons.Js.Id, _mainWindow, _windowsHookManager, _formValidators);
                XmlFileMasks = new StringCollectionEdit(Environment, "Ignored Xml files:",
                    PsiXmlThemedIcons.XmlFile.Id, _mainWindow, _windowsHookManager, _formValidators);
                OtherFileMasks = new StringCollectionEdit(Environment, "Ignored other files:",
                    IdeThemedIcons.TextDocument.Id, _mainWindow, _windowsHookManager, _formValidators);

                BindStringCollectionEditControl(JSFileMasks, SPCAFContribSettingsAccessor.IgnoredJsFileMasks, JSFileMasksPropertyChangedEventHandler);
                BindStringCollectionEditControl(XmlFileMasks, SPCAFContribSettingsAccessor.IgnoredXmlFileMasks, XmlFileMasksPropertyChangedEventHandler);
                BindStringCollectionEditControl(OtherFileMasks, SPCAFContribSettingsAccessor.IgnoredOtherFileMasks, OtherFileMasksPropertyChangedEventHandler);
                
                SplitContainer splitContainer = new SplitContainer();
                splitContainer.FixedPanel = FixedPanel.None;
                splitContainer.Orientation = Orientation.Vertical;
                splitContainer.AutoSize = true;
                splitContainer.Dock = DockStyle.Fill;
                splitContainer.SplitterDistance = splitContainer.ClientSize.Width / 2;
                splitContainer.Panel1.Controls.Add(XmlFileMasks);
                splitContainer.Panel2.Controls.Add(OtherFileMasks);
                
                tablePanel.Controls.Add(JSFileMasks, 0, 0);
                tablePanel.Controls.Add(splitContainer, 0, 1);
            }
        }

        private void BindStringCollectionEditControl(StringCollectionEdit control, Expression<Func<SPCAFContribSettingsKey, IIndexedEntry<string, string>>> keyExpression, PropertyChangedEventHandler handler)
        {
            control.Items.PropertyChanged += handler;
            control.Items.Value = _settings.EnumIndexedValues(keyExpression).ToArray();
            control.Dock = DockStyle.Fill;
        }

        public override bool OnOk()
        {
            ApplyDiff(SPCAFContribSettingsAccessor.IgnoredJsFileMasks, JSFileMasks.Items.Value);
            ApplyDiff(SPCAFContribSettingsAccessor.IgnoredXmlFileMasks, XmlFileMasks.Items.Value);
            ApplyDiff(SPCAFContribSettingsAccessor.IgnoredOtherFileMasks, OtherFileMasks.Items.Value);

            return base.OnOk();
        }

        private void ApplyDiff(Expression<Func<SPCAFContribSettingsKey, IIndexedEntry<string, string>>> keyExpression, string[] newValues)
        {
            HashSet<string> addedAlreadyFileMasks = new HashSet<string>();

            foreach (string indexedValue in _settings.EnumIndexedValues(keyExpression))
            {
                if (!newValues.Contains(indexedValue))
                    _settings.RemoveIndexedValue(keyExpression, indexedValue);
                else
                    addedAlreadyFileMasks.Add(indexedValue);
            }

            foreach (string entryIndex in newValues.Where(x => !addedAlreadyFileMasks.Contains(x)))
                _settings.SetIndexedValue(keyExpression, entryIndex, entryIndex);
        }

        private void JSFileMasksPropertyChangedEventHandler(object sender, PropertyChangedEventArgs args)
        {
            CheckEnteredMask(JSFileMasks.Items.Value, (new_value) => JSFileMasks.Items.Value = new_value);
        }

        private void XmlFileMasksPropertyChangedEventHandler(object sender, PropertyChangedEventArgs args)
        {
            CheckEnteredMask(XmlFileMasks.Items.Value, (new_value) => XmlFileMasks.Items.Value = new_value);
        }

        private void OtherFileMasksPropertyChangedEventHandler(object sender, PropertyChangedEventArgs args)
        {
            CheckEnteredMask(OtherFileMasks.Items.Value, (new_value) => OtherFileMasks.Items.Value = new_value);
        } 

        private void CheckEnteredMask(string[] value, Action<string[]> func)
        {
            string invalidMask = value.FirstOrDefault(mask => !GeneratedUtils.IsValidGeneratedFilesMask(mask));
            if (invalidMask == null)
                return;
            JetBrains.Util.MessageBox.ShowError(string.Format("Pattern \"{0}\" is not valid", invalidMask), "Cannot add generated file mask");
            func(value.Where(s => s != invalidMask).ToArray());
        }
    }
}
