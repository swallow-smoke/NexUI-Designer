using UnityEditor;

namespace emiteat.NexUI.Integrations.Figma
{
    /// <summary>Menu entry for the C5 Figma bridge.</summary>
    public static class FigmaMenu
    {
        [MenuItem("Tools/NexUI/Designer/Advanced/Figma Bridge")]
        public static void Open() => FigmaWindow.Open();
    }
}
