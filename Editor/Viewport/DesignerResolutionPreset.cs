using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public readonly struct DesignerResolutionPreset
    {
        public readonly string Name;
        public readonly Vector2Int Resolution;

        public DesignerResolutionPreset(string name, int width, int height)
        {
            Name = name;
            Resolution = new Vector2Int(width, height);
        }

        public static readonly DesignerResolutionPreset[] Defaults =
        {
            new DesignerResolutionPreset("1920x1080", 1920, 1080),
            new DesignerResolutionPreset("1600x900", 1600, 900),
            new DesignerResolutionPreset("1366x768", 1366, 768),
            new DesignerResolutionPreset("1280x720", 1280, 720),
            new DesignerResolutionPreset("2560x1440", 2560, 1440),
            new DesignerResolutionPreset("Steam Deck", 1280, 800),
            new DesignerResolutionPreset("Mobile Portrait", 1080, 1920),
            new DesignerResolutionPreset("Mobile Landscape", 1920, 1080)
        };
    }
}
