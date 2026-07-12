using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Coordinate conversions between an element's <b>canvas</b> (screen-absolute) rect and its
    /// <b>local</b> rect (relative to its parent's content origin).
    ///
    /// Design note: <see cref="DesignerElementMetadata.rect"/> is stored in absolute canvas space
    /// (the space the viewport renders in) - this is the historical, backwards-compatible
    /// representation and the persisted source of truth, so no data migration of positions is
    /// required. "Local position" is therefore a <i>computed</i> view (canvas minus parent's
    /// canvas origin, minus the parent's left/top content padding). The Inspector edits local
    /// position through <see cref="SetLocalPosition"/>, and reparent commands use these helpers to
    /// keep an element visually fixed on screen (canvas rect unchanged) while its local position
    /// is recomputed against the new parent.
    /// </summary>
    public static class DesignerCoordinateUtility
    {
        /// <summary>Canvas (screen-absolute) rect of an element - identical to its stored rect in the current representation.</summary>
        public static Rect GetCanvasRect(DesignerMetadataAsset asset, DesignerElementMetadata element)
            => element != null ? element.rect : default;

        /// <summary>The canvas-space origin (top-left) of a parent's content box, accounting for its left/top content padding. Root parent ⇒ (0,0).</summary>
        public static Vector2 GetParentContentOrigin(DesignerMetadataAsset asset, DesignerElementMetadata element)
        {
            if (asset == null || element == null || string.IsNullOrEmpty(element.parentId))
                return Vector2.zero;
            var parent = asset.Find(element.parentId);
            if (parent == null) return Vector2.zero;
            var origin = parent.rect.position;
            if (parent.contentPadding != null)
                origin += new Vector2(parent.contentPadding.left, parent.contentPadding.top);
            return origin;
        }

        /// <summary>Position of an element relative to its parent's content origin (root elements: same as canvas position).</summary>
        public static Vector2 GetLocalPosition(DesignerMetadataAsset asset, DesignerElementMetadata element)
        {
            if (element == null) return Vector2.zero;
            return element.rect.position - GetParentContentOrigin(asset, element);
        }

        /// <summary>Canvas position that a given local position maps to under <paramref name="element"/>'s current parent.</summary>
        public static Vector2 LocalToCanvas(DesignerMetadataAsset asset, DesignerElementMetadata element, Vector2 local)
            => local + GetParentContentOrigin(asset, element);

        /// <summary>Local position that a given canvas position maps to under <paramref name="element"/>'s current parent.</summary>
        public static Vector2 CanvasToLocal(DesignerMetadataAsset asset, DesignerElementMetadata element, Vector2 canvas)
            => canvas - GetParentContentOrigin(asset, element);

        /// <summary>Local position under a hypothetical <paramref name="parent"/> (used when previewing a reparent).</summary>
        public static Vector2 CanvasToLocalUnder(DesignerMetadataAsset asset, DesignerElementMetadata parent, Vector2 canvas)
        {
            if (parent == null) return canvas;
            var origin = parent.rect.position;
            if (parent.contentPadding != null)
                origin += new Vector2(parent.contentPadding.left, parent.contentPadding.top);
            return canvas - origin;
        }

        /// <summary>Returns a copy of the element's rect placed at the given local position (canvas rect resolved through its parent).</summary>
        public static Rect WithLocalPosition(DesignerMetadataAsset asset, DesignerElementMetadata element, Vector2 local)
        {
            var r = element != null ? element.rect : default;
            r.position = LocalToCanvas(asset, element, local);
            return r;
        }

        /// <summary>Union canvas-space bounding box of a set of elements (empty ⇒ zero rect).</summary>
        public static Rect GetCanvasBounds(System.Collections.Generic.IReadOnlyList<DesignerElementMetadata> elements)
        {
            if (elements == null || elements.Count == 0) return default;
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            var any = false;
            for (int i = 0; i < elements.Count; i++)
            {
                var e = elements[i];
                if (e == null) continue;
                any = true;
                min = Vector2.Min(min, e.rect.min);
                max = Vector2.Max(max, e.rect.max);
            }
            return any ? new Rect(min, max - min) : default;
        }
    }
}
