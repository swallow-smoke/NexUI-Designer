using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.UI.Shell;
using UnityEditor;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerWindow : EditorWindow
    {
        public NexUIDesignerContext Context { get; private set; }

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

            rootVisualElement.Add(new NexUIDesignerShell(Context));
        }
    }
}
