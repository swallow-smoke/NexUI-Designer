using UnityEditor;

namespace emiteat.NexUI.Designer.Editor.Localization
{
    public static class DesignerLocalizationMenu
    {
        [MenuItem("Tools/NexUI/Designer/Language/Korean", true)]
        private static bool KoreanValidate() => true;

        [MenuItem("Tools/NexUI/Designer/Language/English", true)]
        private static bool EnglishValidate() => true;
    }
}
