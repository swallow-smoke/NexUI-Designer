using System.Collections.Generic;
using emiteat.NexUI.Accessibility;

namespace emiteat.NexUI.Designer.Editor.Accessibility
{
    /// <summary>
    /// Describes the effects that an <see cref="UIAccessibilityPreference"/> has on the
    /// UI, for the accessibility preview panel (§19).
    /// </summary>
    public static class AccessibilityService
    {
        public static List<string> Effects(UIAccessibilityPreference pref)
        {
            var effects = new List<string>();
            if (pref == null) return effects;

            if (pref.reduceMotion)
            {
                effects.Add("Shake motion disabled");
                effects.Add("Popup scale reduced");
                effects.Add("Fade-centered transitions");
                effects.Add("Layout motion simplified");
            }
            if (pref.highContrast)
            {
                effects.Add("High-contrast theme palette applied");
                effects.Add("Focus outlines strengthened");
            }
            if (pref.largeText)
            {
                effects.Add("Text scale increased");
                effects.Add("Watch for text overflow on fixed-size labels");
            }
            if (effects.Count == 0)
                effects.Add("No accessibility overrides active");
            return effects;
        }
    }
}
