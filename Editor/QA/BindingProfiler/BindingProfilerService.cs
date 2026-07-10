using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.BindingProfiler
{
    /// <summary>
    /// Static analysis of a screen's bindings for update-cost risk. Keys whose names
    /// suggest per-frame values (position, time, velocity, delta, mouse, fps) are flagged
    /// as poor fits for UI binding. Also reports total binding count.
    /// </summary>
    public static class BindingProfilerService
    {
        private static readonly string[] HotHints =
            { "position", "time", "velocity", "delta", "mouse", "fps", "frame", "elapsed" };

        public static List<string> Analyze(DesignerMetadataAsset asset, out int bindingCount)
        {
            var messages = new List<string>();
            bindingCount = 0;
            if (asset == null) return messages;

            foreach (var e in asset.elements)
            {
                if (e?.binding == null) continue;
                CheckKey(e.elementId, "textKey", e.binding.textKey, ref bindingCount, messages);
                CheckKey(e.elementId, "valueKey", e.binding.valueKey, ref bindingCount, messages);
                CheckKey(e.elementId, "visibilityKey", e.binding.visibilityKey, ref bindingCount, messages);
                CheckKey(e.elementId, "interactableKey", e.binding.interactableKey, ref bindingCount, messages);
                CheckKey(e.elementId, "classKey", e.binding.classKey, ref bindingCount, messages);
                CheckKey(e.elementId, "commandKey", e.binding.commandKey, ref bindingCount, messages);
            }
            return messages;
        }

        private static void CheckKey(string elementId, string field, string key, ref int count, List<string> messages)
        {
            if (string.IsNullOrEmpty(key)) return;
            count++;
            string lower = key.ToLowerInvariant();
            foreach (var hint in HotHints)
                if (lower.Contains(hint))
                {
                    messages.Add($"• {elementId}.{field} '{key}' may update very frequently — not ideal for UI binding.");
                    break;
                }
        }
    }
}
