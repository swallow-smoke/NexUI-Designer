using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Profiler
{
    /// <summary>
    /// Builds a <see cref="UIProfilerSnapshot"/> from Designer metadata (static counts).
    /// Live timings (screen open/close/theme-apply ms) are only populated when captured
    /// from a running UIManager; this design-time capture fills the structural counts.
    /// </summary>
    public static class UIProfilerService
    {
        public static UIProfilerSnapshot CaptureStatic(DesignerMetadataAsset asset)
        {
            var s = new UIProfilerSnapshot { openedScreenCount = asset != null ? 1 : 0 };
            if (asset == null) return s;

            s.elementCount = asset.elements.Count;
            int bindings = 0, motions = 0;
            foreach (var e in asset.elements)
            {
                if (e?.binding != null)
                {
                    if (!string.IsNullOrEmpty(e.binding.textKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.valueKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.visibilityKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.interactableKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.classKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.commandKey)) bindings++;
                }
                if (e?.motion != null && !string.IsNullOrEmpty(e.motion.motionId)) motions++;
            }
            s.bindingCount = bindings;
            s.activeMotionCount = motions;
            return s;
        }

        public static string ToJson(UIProfilerSnapshot snapshot) => JsonUtility.ToJson(snapshot, true);

        public static List<string> Warnings(UIProfilerSnapshot s)
        {
            var messages = new List<string>();
            if (s.elementCount > 200) messages.Add($"• high element count: {s.elementCount}");
            if (s.bindingCount > 150) messages.Add($"• high binding count: {s.bindingCount}");
            if (s.activeMotionCount > 32) messages.Add($"• high active motion count: {s.activeMotionCount}");
            return messages;
        }
    }
}
