using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor
{
    public static class NexUIDesignerMenu
    {
        [MenuItem("Tools/NexUI/Designer")]
        public static void OpenDesigner() => NexUIDesigner.Open();

        [MenuItem("Tools/NexUI/Designer/Open Selected Screen")]
        public static void OpenSelectedScreen()
        {
            var definition = Selection.activeObject as UIScreenDefinition;
            if (definition != null) NexUIDesigner.Open(definition);
            else NexUIDesigner.Open();
        }

        [MenuItem("Tools/NexUI/Designer/Rebuild Preview")]
        public static void RebuildPreview() => NexUIDesigner.RebuildPreview();

        [MenuItem("Tools/NexUI/Designer/Validate Current Screen")]
        public static void ValidateCurrent() => NexUIDesigner.ValidateCurrent();

        [MenuItem("Tools/NexUI/Designer/Save Current Screen")]
        public static void SaveCurrent() => NexUIDesigner.SaveCurrent();

        [MenuItem("Tools/NexUI/Designer/Language/Korean")]
        public static void Korean() => DesignerLocalization.SetLanguage(DesignerLanguage.Korean);

        [MenuItem("Tools/NexUI/Designer/Language/English")]
        public static void English() => DesignerLocalization.SetLanguage(DesignerLanguage.English);
    }
}
