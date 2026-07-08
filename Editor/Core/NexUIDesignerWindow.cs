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
        private NexUIDesignerViewport _viewport;
        private NexUIDesignerInspector _inspector;
        private NexUIDesignerValidationPanel _validation;
        private NexUIDesignerStatePanel _state;
        private NexUIDesignerCommandPanel _commands;
        private NexUIDesignerScreenGraphPanel _screenGraph;

        private void OnEnable()
        {
            Context = new NexUIDesignerContext();
        }

        private void OnDisable()
        {
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
            _viewport = new NexUIDesignerViewport(Context);
            _inspector = new NexUIDesignerInspector(Context);
            _validation = new NexUIDesignerValidationPanel(Context);
            _state = new NexUIDesignerStatePanel(Context);
            _commands = new NexUIDesignerCommandPanel(Context);
            _screenGraph = new NexUIDesignerScreenGraphPanel(Context);

            rootVisualElement.Add(_toolbar);

            var body = new VisualElement { name = "DesignerBody" };
            body.AddToClassList("nexui-designer-body");
            body.style.flexDirection = FlexDirection.Row;
            body.style.flexGrow = 1;
            body.style.flexShrink = 1;

            var left = new VisualElement { name = "DesignerLeft" };
            left.AddToClassList("nexui-side-panel");
            left.AddToClassList("nexui-left-panel");
            left.style.width = 160;
            left.style.flexShrink = 0;
            left.Add(_palette);
            left.Add(_hierarchy);

            var center = new VisualElement { name = "DesignerCenter" };
            center.AddToClassList("nexui-center-panel");
            center.style.flexGrow = 1;
            center.style.flexShrink = 1;
            center.Add(_viewport);

            var right = new VisualElement { name = "DesignerRight" };
            right.AddToClassList("nexui-side-panel");
            right.AddToClassList("nexui-right-panel");
            right.style.width = 240;
            right.style.flexShrink = 0;
            right.Add(_inspector);
            right.Add(_validation);

            body.Add(left);
            body.Add(center);
            body.Add(right);

            var bottom = new VisualElement { name = "DesignerBottom" };
            bottom.AddToClassList("nexui-bottom-panel");
            bottom.style.flexDirection = FlexDirection.Row;
            bottom.Add(_state);
            bottom.Add(_commands);
            bottom.Add(_screenGraph);

            rootVisualElement.Add(body);
            rootVisualElement.Add(bottom);
        }
    }
}
