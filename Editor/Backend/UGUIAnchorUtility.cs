using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Backend
{
    /// <summary>
    /// Applies <see cref="DesignerAnchorPreset"/> values to a uGUI <see cref="RectTransform"/>.
    /// Shared by the live preview backend and the prefab serializer so anchoring is
    /// identical whether the user is editing in the viewport or saving to disk.
    /// </summary>
    public static class UGUIAnchorUtility
    {
        /// <summary>
        /// Sets the anchor / pivot for the given preset. For non-stretch presets the
        /// current size (sizeDelta) is preserved; Stretch clears offsets so the element
        /// fills its parent.
        /// </summary>
        public static void Apply(RectTransform rt, DesignerAnchorPreset preset)
        {
            if (rt == null) return;

            var size = rt.rect.size;
            var anchoredPosition = rt.anchoredPosition;

            switch (preset)
            {
                case DesignerAnchorPreset.TopLeft: Set(rt, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f)); break;
                case DesignerAnchorPreset.Top: Set(rt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f)); break;
                case DesignerAnchorPreset.TopRight: Set(rt, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f)); break;
                case DesignerAnchorPreset.Left: Set(rt, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f)); break;
                case DesignerAnchorPreset.Center: Set(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)); break;
                case DesignerAnchorPreset.Right: Set(rt, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f)); break;
                case DesignerAnchorPreset.BottomLeft: Set(rt, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)); break;
                case DesignerAnchorPreset.Bottom: Set(rt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)); break;
                case DesignerAnchorPreset.BottomRight: Set(rt, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f)); break;
                case DesignerAnchorPreset.Stretch:
                    rt.anchorMin = new Vector2(0f, 0f);
                    rt.anchorMax = new Vector2(1f, 1f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                    return;
            }

            // Preserve visual size / position for non-stretch presets.
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPosition;
        }

        private static void Set(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
        }
    }
}
