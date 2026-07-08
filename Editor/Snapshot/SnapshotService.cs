using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Snapshot
{
    /// <summary>
    /// Captures a <see cref="UISnapshot"/> from Designer metadata and compares two
    /// snapshots (changed / removed / added elements) for snapshot testing and diffing.
    /// Position/size are only populated when captured from a live runtime surface;
    /// metadata-based capture fills the fields it can (id, visible, interactable, text, classes).
    /// </summary>
    public static class SnapshotService
    {
        public static UISnapshot Capture(DesignerMetadataAsset asset, string variantId = null, string themeId = null)
        {
            var snap = new UISnapshot
            {
                snapshotId = Guid.NewGuid().ToString("N").Substring(0, 8),
                screenId = asset.screenId,
                variantId = variantId,
                themeId = themeId
            };
            foreach (var e in asset.elements)
            {
                if (e == null) continue;
                snap.elements.Add(new UISnapshotElement
                {
                    elementId = e.elementId,
                    visible = !e.hiddenInDesigner,
                    interactable = !e.locked,
                    text = e.binding != null ? e.binding.textKey : null,
                    classes = new List<string>(e.classes)
                });
            }
            return snap;
        }

        /// <summary>Diffs baseline → current. Empty list means no change.</summary>
        public static List<string> Compare(UISnapshot baseline, UISnapshot current)
        {
            var messages = new List<string>();
            if (baseline == null || current == null)
            {
                messages.Add("• one of the snapshots is null");
                return messages;
            }

            foreach (var b in baseline.elements)
            {
                var c = current.Find(b.elementId);
                if (c == null) { messages.Add($"- removed: {b.elementId}"); continue; }

                if (b.visible != c.visible) messages.Add($"~ {b.elementId}.visible: {b.visible} → {c.visible}");
                if (b.interactable != c.interactable) messages.Add($"~ {b.elementId}.interactable: {b.interactable} → {c.interactable}");
                if (b.text != c.text) messages.Add($"~ {b.elementId}.text: {b.text} → {c.text}");
                if (!Approx(b.value, c.value)) messages.Add($"~ {b.elementId}.value: {b.value} → {c.value}");
                if (b.position != c.position) messages.Add($"~ {b.elementId}.position: {b.position} → {c.position}");
                if (b.size != c.size) messages.Add($"~ {b.elementId}.size: {b.size} → {c.size}");
                if (!ClassesEqual(b.classes, c.classes)) messages.Add($"~ {b.elementId}.classes changed");
            }

            foreach (var c in current.elements)
                if (baseline.Find(c.elementId) == null)
                    messages.Add($"+ added: {c.elementId}");

            return messages;
        }

        private static bool Approx(float a, float b) => UnityEngine.Mathf.Approximately(a, b);

        private static bool ClassesEqual(List<string> a, List<string> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (!b.Contains(a[i])) return false;
            return true;
        }
    }
}
