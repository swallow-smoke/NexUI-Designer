using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor
{
    public static class NexUIDesigner
    {
        private static NexUIDesignerWindow _window;

        public static NexUIDesignerWindow Open()
        {
            _window = EditorWindow.GetWindow<NexUIDesignerWindow>();
            _window.titleContent.text = DesignerLocalization.T("designer.title");
            _window.Show();
            return _window;
        }

        public static NexUIDesignerWindow Open(UIScreenDefinition definition)
        {
            var window = Open();
            window.Context.Open(definition);
            return window;
        }

        public static void Ping(UIScreenDefinition definition)
        {
            if (definition == null) return;
            EditorGUIUtility.PingObject(definition);
            Selection.activeObject = definition;
        }

        public static void RebuildPreview() => Open().Context.RebuildPreview();
        public static void ValidateCurrent() => Open().Context.Validate();
        public static void SaveCurrent() => Open().Context.Save();
    }
}
