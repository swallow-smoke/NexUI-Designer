using emiteat.NexUI.Designer.Editor.Inspectors;
using emiteat.NexUI.Designer.Editor.Panels;
using emiteat.NexUI.Designer.Editor.UI.Controls;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.UI.Shell
{
    public sealed class NexUIRightInspector : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly NexUITabBar<DesignerInspectorTab> _tabs;
        private readonly ScrollView _host;
        private bool _showingScreen;

        public NexUIRightInspector(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-right-inspector");

            _tabs = new NexUITabBar<DesignerInspectorTab>(context.InspectorTab, context.SetInspectorTab,
                (DesignerInspectorTab.Design, "Design", "Layout, appearance, and accessibility."),
                (DesignerInspectorTab.Prototype, "Prototype", "Binding, command, state, and localization."),
                (DesignerInspectorTab.Motion, "Motion", "Motion presets, clips, and graph tools."));
            Add(_tabs);

            _host = new ScrollView();
            _host.AddToClassList("nexui-inspector-host");
            Add(_host);

            context.UIStateChanged += Refresh;
            context.MultiSelectionChanged += _ => RefreshIfModeChanged();
            DesignerEditMode.Changed += _ => Refresh();
            Refresh();
        }

        private void RefreshIfModeChanged()
        {
            var screen = _context.SelectedElements.Count == 0;
            if (screen != _showingScreen)
                Refresh();
        }

        private void Refresh()
        {
            _tabs.SetCurrent(_context.InspectorTab);
            _host.Clear();
            _showingScreen = _context.SelectedElements.Count == 0;

            if (_showingScreen)
            {
                AddScreenInspector();
                return;
            }

            switch (_context.InspectorTab)
            {
                case DesignerInspectorTab.Prototype:
                    AddPrototypeInspector();
                    break;
                case DesignerInspectorTab.Motion:
                    AddMotionInspector();
                    break;
                default:
                    AddDesignInspector();
                    break;
            }
        }

        private void AddScreenInspector()
        {
            AddTitle("Screen");
            _host.Add(new ScreenDefinitionInspector(_context));
            _host.Add(new ValidationInspector(_context));
            if (DesignerEditMode.IsAdvanced)
                _host.Add(new PolicyInspector(_context));
        }

        private void AddDesignInspector()
        {
            AddTitle(_context.SelectedElements.Count > 1 ? _context.SelectedElements.Count + " Elements Selected" : "Selection");
            _host.Add(new MultiSelectionInspector(_context));
            _host.Add(new LayoutInspector(_context));
            _host.Add(new AutoLayoutInspector(_context));
            _host.Add(new ConstraintsInspector(_context));
            _host.Add(new StyleInspector(_context));
            _host.Add(new AccessibilityInspector(_context));
            if (DesignerEditMode.IsAdvanced)
                _host.Add(new CapabilityInspector(_context));
        }

        private void AddPrototypeInspector()
        {
            AddTitle("Prototype");
            _host.Add(new BindingInspector(_context));
            _host.Add(new NexUIDesignerStatePanel(_context));
            _host.Add(new NexUIDesignerCommandPanel(_context));
            _host.Add(new ValidationInspector(_context));
            if (DesignerEditMode.IsAdvanced)
                _host.Add(new PolicyInspector(_context));
        }

        private void AddMotionInspector()
        {
            AddTitle("Motion");
            _host.Add(new MotionInspector(_context));
            _host.Add(new ThemeInspector(_context));
        }

        private void AddTitle(string text)
        {
            var label = new Label(text);
            label.AddToClassList("nexui-inspector-title");
            _host.Add(label);
        }
    }
}
