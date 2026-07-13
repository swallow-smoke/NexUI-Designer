using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.FocusNav
{
    /// <summary>
    /// Computes nearest-neighbor directional links for every element on a screen, using the
    /// Designer canvas's own known coordinate space (<c>DesignerElementMetadata.rect</c>, Y
    /// increasing downward - UI Toolkit layout convention). Kept Editor-side rather than a
    /// generic Runtime utility because the Y-axis convention differs across render backends
    /// (uGUI's RectTransform is Y-up by default) and guessing wrong there would silently produce
    /// backwards Up/Down links - the Designer authors links once, in a space it fully controls,
    /// and <c>UIFocusNavigationGraph</c> just follows whatever was authored.
    /// </summary>
    public static class FocusNavigationAutoLayout
    {
        public readonly struct ElementBounds
        {
            public readonly string ElementId;
            public readonly Vector2 Center;

            public ElementBounds(string elementId, Vector2 center)
            {
                ElementId = elementId;
                Center = center;
            }
        }

        /// <summary>For each element, finds the nearest other element in each of the four directions (within a ~60-degree cone), weighting off-axis distance more heavily so a slightly-offset element in the right general direction still wins over a far one directly on-axis.</summary>
        public static Dictionary<string, DesignerFocusLinks> GenerateNearest(IReadOnlyList<ElementBounds> elements)
        {
            var result = new Dictionary<string, DesignerFocusLinks>();
            foreach (var element in elements)
            {
                result[element.ElementId] = new DesignerFocusLinks
                {
                    UpElementId = FindNearest(element, elements, new Vector2(0f, -1f)),
                    DownElementId = FindNearest(element, elements, new Vector2(0f, 1f)),
                    LeftElementId = FindNearest(element, elements, new Vector2(-1f, 0f)),
                    RightElementId = FindNearest(element, elements, new Vector2(1f, 0f))
                };
            }
            return result;
        }

        private static string FindNearest(ElementBounds from, IReadOnlyList<ElementBounds> all, Vector2 direction)
        {
            string bestId = null;
            var bestScore = float.MaxValue;

            foreach (var candidate in all)
            {
                if (candidate.ElementId == from.ElementId) continue;
                var offset = candidate.Center - from.Center;
                if (offset.sqrMagnitude < 0.0001f) continue;

                var alignment = Vector2.Dot(offset.normalized, direction);
                if (alignment < 0.5f) continue; // outside the ~60-degree cone in this direction

                var score = offset.magnitude / alignment; // off-axis distance is penalized more
                if (score < bestScore)
                {
                    bestScore = score;
                    bestId = candidate.ElementId;
                }
            }

            return bestId;
        }
    }

    /// <summary>Plain result bundle for <see cref="FocusNavigationAutoLayout.GenerateNearest"/> - a pure computation result, independent of <see cref="DesignerFocusMetadata"/>'s serialized/mutable shape so this utility stays testable without touching any element's actual metadata.</summary>
    public struct DesignerFocusLinks
    {
        public string UpElementId;
        public string DownElementId;
        public string LeftElementId;
        public string RightElementId;
    }
}
