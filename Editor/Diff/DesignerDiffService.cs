using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Diff
{
    /// <summary>
    /// Human-readable diffing of two <see cref="DesignerMetadataAsset"/> states
    /// (before/after): added/removed elements plus per-element binding, class, motion
    /// and theme changes. Sub-objects are compared via <see cref="JsonUtility"/> so the
    /// diff stays correct even as their fields evolve.
    /// </summary>
    public static class DesignerDiffService
    {
        public static List<string> DiffMetadata(DesignerMetadataAsset before, DesignerMetadataAsset after)
        {
            var messages = new List<string>();
            if (before == null || after == null)
            {
                messages.Add("• one of the assets is null");
                return messages;
            }

            if (before.screenId != after.screenId)
                messages.Add($"~ screenId: {before.screenId} → {after.screenId}");

            foreach (var b in before.elements)
            {
                var a = after.Find(b.elementId);
                if (a == null) { messages.Add($"- removed element: {b.elementId}"); continue; }
                DiffElement(b, a, messages);
            }
            foreach (var a in after.elements)
                if (before.Find(a.elementId) == null)
                    messages.Add($"+ added element: {a.elementId}");

            DiffList("variant", Ids(before.variants, v => v.variantId), Ids(after.variants, v => v.variantId), messages);
            DiffList("responsive rule", Ids(before.responsiveRules, r => r.ruleId), Ids(after.responsiveRules, r => r.ruleId), messages);

            return messages;
        }

        private static void DiffElement(DesignerElementMetadata b, DesignerElementMetadata a, List<string> messages)
        {
            if (JsonUtility.ToJson(b.binding) != JsonUtility.ToJson(a.binding))
                messages.Add($"~ {b.elementId}.binding changed");
            if (JsonUtility.ToJson(b.motion) != JsonUtility.ToJson(a.motion))
                messages.Add($"~ {b.elementId}.motion changed");
            if (JsonUtility.ToJson(b.theme) != JsonUtility.ToJson(a.theme))
                messages.Add($"~ {b.elementId}.theme changed");
            if (!SameSet(b.classes, a.classes))
                messages.Add($"~ {b.elementId}.classes changed");
            if (b.parentId != a.parentId)
                messages.Add($"~ {b.elementId}.parent: {b.parentId} → {a.parentId}");
        }

        private static List<string> Ids<T>(List<T> list, System.Func<T, string> selector)
        {
            var ids = new List<string>();
            foreach (var item in list) ids.Add(selector(item));
            return ids;
        }

        private static void DiffList(string label, List<string> before, List<string> after, List<string> messages)
        {
            foreach (var b in before) if (!after.Contains(b)) messages.Add($"- removed {label}: {b}");
            foreach (var a in after) if (!before.Contains(a)) messages.Add($"+ added {label}: {a}");
        }

        private static bool SameSet(List<string> a, List<string> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++) if (!b.Contains(a[i])) return false;
            return true;
        }
    }
}
