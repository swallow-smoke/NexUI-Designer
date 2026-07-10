using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.Panels;
using emiteat.NexUI.Designer.Editor.Viewport;
using UnityEditor;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerWindow : EditorWindow
    {
        public NexUIDesignerContext Context { get; private set; }

        private NexUIDesignerToolbar _toolbar;
        private NexUIDesignerPalette _palette;
        private NexUIDesignerHierarchy _hierarchy;
        private NexUIDesignerAlignPanel _alignPanel;
        private NexUIDesignerViewport _viewport;
        private NexUIDesignerInspector _inspector;
        private NexUIDesignerValidationPanel _validation;
        private NexUIDesignerHistoryPanel _history;
        private NexUIDesignerStatePanel _state;
        private NexUIDesignerCommandPanel _commands;
        private NexUIDesignerScreenGraphPanel _screenGraph;

        private void OnEnable()
        {
            Context = new NexUIDesignerContext();
            // Every localized string (panel titles, tooltips, ...) is only read at construction
            // time, so re-run Rebuild() on language switch instead of requiring the window to be
            // closed and reopened.
            DesignerLocalization.LanguageChanged += Rebuild;
        }

        private void OnDisable()
        {
            DesignerLocalization.LanguageChanged -= Rebuild;
            Context?.Dispose();
        }

        public void CreateGUI() => Rebuild();

        private void Rebuild()
        {
            titleContent.text = DesignerLocalization.T("designer.title");
            rootVisualElement.Clear();
            rootVisualElement.AddToClassList("nexui-designer-root");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.emiteat.nexui.designer/Editor/Styles/NexUIDesigner.uss");
            if (styleSheet != null && !rootVisualElement.styleSheets.Contains(styleSheet))
                rootVisualElement.styleSheets.Add(styleSheet);
            if (EditorStyles.label.font != null)
                rootVisualElement.style.unityFont = EditorStyles.label.font;

            _toolbar = new NexUIDesignerToolbar(Context);
            _palette = new NexUIDesignerPalette(Context);
            _hierarchy = new NexUIDesignerHierarchy(Context);
            _alignPanel = new NexUIDesignerAlignPanel(Context);
            _viewport = new NexUIDesignerViewport(Context);
            _inspector = new NexUIDesignerInspector(Context);
            _validation = new NexUIDesignerValidationPanel(Context);
            _history = new NexUIDesignerHistoryPanel(Context);
            _state = new NexUIDesignerStatePanel(Context);
            _commands = new NexUIDesignerCommandPanel(Context);
            _screenGraph = new NexUIDesignerScreenGraphPanel(Context);

            var componentsColumn = new VisualElement { name = "DesignerComponentsColumn" };
            componentsColumn.style.flexGrow = 1;
            componentsColumn.style.flexShrink = 1;
            componentsColumn.style.minWidth = 100;
            componentsColumn.Add(_palette);
            componentsColumn.Add(_hierarchy);

            var left = new VisualElement { name = "DesignerLeft" };
            left.AddToClassList("nexui-side-panel");
            left.AddToClassList("nexui-left-panel");
            left.style.flexDirection = FlexDirection.Row;
            left.Add(componentsColumn);
            left.Add(_alignPanel);

            var center = new VisualElement { name = "DesignerCenter" };
            center.AddToClassList("nexui-center-panel");
            center.style.flexGrow = 1;
            center.style.flexShrink = 1;
            center.Add(_viewport);

            var right = new VisualElement { name = "DesignerRight" };
            right.AddToClassList("nexui-side-panel");
            right.AddToClassList("nexui-right-panel");
            right.Add(_inspector);
            right.Add(_validation);
            right.Add(_history);

            // All three main columns and the bottom dock are user-resizable: nest two
            // horizontal splits (left | center|right, then center | right) inside one
            // vertical split (body | bottom), each remembering its own drag position
            // per-window via TwoPaneSplitView's built-in EditorPrefs persistence (viewDataKey).
            var centerRightSplit = new TwoPaneSplitView(1, 240, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "CenterRightSplit",
                viewDataKey = "NexUI.Designer.Split.CenterRight"
            };
            centerRightSplit.style.flexGrow = 1;
            centerRightSplit.Add(center);
            centerRightSplit.Add(right);

            var leftBodySplit = new TwoPaneSplitView(0, 220, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "LeftBodySplit",
                viewDataKey = "NexUI.Designer.Split.LeftBody"
            };
            leftBodySplit.AddToClassList("nexui-designer-body");
            leftBodySplit.style.flexGrow = 1;
            leftBodySplit.Add(left);
            leftBodySplit.Add(centerRightSplit);

            var bottom = new VisualElement { name = "DesignerBottom" };
            bottom.AddToClassList("nexui-bottom-panel");
            bottom.style.flexDirection = FlexDirection.Row;
            bottom.Add(_state);
            bottom.Add(_commands);
            bottom.Add(_screenGraph);

            var bodyBottomSplit = new TwoPaneSplitView(1, 64, TwoPaneSplitViewOrientation.Vertical)
            {
                name = "BodyBottomSplit",
                viewDataKey = "NexUI.Designer.Split.BodyBottom"
            };
            bodyBottomSplit.style.flexGrow = 1;
            bodyBottomSplit.Add(leftBodySplit);
            bodyBottomSplit.Add(bottom);

            // Toolbar is also a resizable pane now (drag its bottom edge down to see more of a
            // long Screen/Metadata field, or up to reclaim canvas space) instead of a fixed height.
            var toolbarBodySplit = new TwoPaneSplitView(0, 72, TwoPaneSplitViewOrientation.Vertical)
            {
                name = "ToolbarBodySplit",
                viewDataKey = "NexUI.Designer.Split.ToolbarBody"
            };
            toolbarBodySplit.style.flexGrow = 1;
            toolbarBodySplit.Add(_toolbar);
            toolbarBodySplit.Add(bodyBottomSplit);

            rootVisualElement.Add(toolbarBodySplit);
        }
    }
}
