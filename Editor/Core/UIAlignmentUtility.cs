using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Pure alignment/distribution math for Designer elements. Every method returns the new
    /// rect for each element via a dictionary rather than mutating anything directly, so the
    /// caller (<see cref="NexUIDesignerContext"/>) stays the single place that records Undo and
    /// marks the metadata asset dirty.
    ///
    /// TODO: reference is currently always the bounding box of the selection (or the canvas
    /// resolution for a single element). Figma-style "key object" alignment (align everything
    /// to the last-selected element instead of the bounding box) is intentionally out of scope
    /// for this pass.
    /// </summary>
    public static class UIAlignmentUtility
    {
        public static Rect GetBounds(IReadOnlyList<DesignerElementMetadata> elements)
        {
            if (elements == null || elements.Count == 0) return default;
            var r = elements[0].rect;
            var min = new Vector2(r.xMin, r.yMin);
            var max = new Vector2(r.xMax, r.yMax);
            for (int i = 1; i < elements.Count; i++)
            {
                var er = elements[i].rect;
                min.x = Mathf.Min(min.x, er.xMin);
                min.y = Mathf.Min(min.y, er.yMin);
                max.x = Mathf.Max(max.x, er.xMax);
                max.y = Mathf.Max(max.y, er.yMax);
            }
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        public static Dictionary<DesignerElementMetadata, Rect> AlignLeft(IReadOnlyList<DesignerElementMetadata> elements, Rect bounds)
            => Apply(elements, r => new Rect(bounds.xMin, r.y, r.width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> AlignCenterX(IReadOnlyList<DesignerElementMetadata> elements, Rect bounds)
            => Apply(elements, r => new Rect(bounds.center.x - r.width * 0.5f, r.y, r.width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> AlignRight(IReadOnlyList<DesignerElementMetadata> elements, Rect bounds)
            => Apply(elements, r => new Rect(bounds.xMax - r.width, r.y, r.width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> AlignTop(IReadOnlyList<DesignerElementMetadata> elements, Rect bounds)
            => Apply(elements, r => new Rect(r.x, bounds.yMin, r.width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> AlignCenterY(IReadOnlyList<DesignerElementMetadata> elements, Rect bounds)
            => Apply(elements, r => new Rect(r.x, bounds.center.y - r.height * 0.5f, r.width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> AlignBottom(IReadOnlyList<DesignerElementMetadata> elements, Rect bounds)
            => Apply(elements, r => new Rect(r.x, bounds.yMax - r.height, r.width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> DistributeHorizontal(IReadOnlyList<DesignerElementMetadata> elements)
        {
            var result = new Dictionary<DesignerElementMetadata, Rect>();
            if (elements == null || elements.Count < 3) return result;

            var ordered = elements.OrderBy(e => e.rect.x).ToList();
            var first = ordered[0].rect;
            var last = ordered[ordered.Count - 1].rect;
            var totalWidth = ordered.Sum(e => e.rect.width);
            var span = last.xMax - first.xMin;
            var gap = (span - totalWidth) / (ordered.Count - 1);

            var cursor = first.xMin;
            foreach (var e in ordered)
            {
                var r = e.rect;
                r.x = cursor;
                result[e] = r;
                cursor += r.width + gap;
            }
            return result;
        }

        public static Dictionary<DesignerElementMetadata, Rect> DistributeVertical(IReadOnlyList<DesignerElementMetadata> elements)
        {
            var result = new Dictionary<DesignerElementMetadata, Rect>();
            if (elements == null || elements.Count < 3) return result;

            var ordered = elements.OrderBy(e => e.rect.y).ToList();
            var first = ordered[0].rect;
            var last = ordered[ordered.Count - 1].rect;
            var totalHeight = ordered.Sum(e => e.rect.height);
            var span = last.yMax - first.yMin;
            var gap = (span - totalHeight) / (ordered.Count - 1);

            var cursor = first.yMin;
            foreach (var e in ordered)
            {
                var r = e.rect;
                r.y = cursor;
                result[e] = r;
                cursor += r.height + gap;
            }
            return result;
        }

        public static Dictionary<DesignerElementMetadata, Rect> MatchWidth(IReadOnlyList<DesignerElementMetadata> elements, float width)
            => Apply(elements, r => new Rect(r.x, r.y, width, r.height));

        public static Dictionary<DesignerElementMetadata, Rect> MatchHeight(IReadOnlyList<DesignerElementMetadata> elements, float height)
            => Apply(elements, r => new Rect(r.x, r.y, r.width, height));

        public static Dictionary<DesignerElementMetadata, Rect> MatchSize(IReadOnlyList<DesignerElementMetadata> elements, Vector2 size)
            => Apply(elements, r => new Rect(r.x, r.y, size.x, size.y));

        private static Dictionary<DesignerElementMetadata, Rect> Apply(
            IReadOnlyList<DesignerElementMetadata> elements, System.Func<Rect, Rect> project)
        {
            var result = new Dictionary<DesignerElementMetadata, Rect>();
            if (elements == null) return result;
            foreach (var e in elements)
                result[e] = project(e.rect);
            return result;
        }
    }
}
