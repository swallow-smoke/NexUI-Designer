using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer;
using UnityEngine;

namespace emiteat.NexUI.Integrations.Figma
{
    /// <summary>Maps a fetched Figma document's first frame into Designer metadata without writing backend assets.</summary>
    public static class FigmaDocumentImporter
    {
        [Serializable] private sealed class FileResponse { public FigmaNode document; }
        [Serializable] private sealed class FigmaNode
        {
            public string id;
            public string name;
            public string type;
            public string characters;
            public string layoutMode;
            public float itemSpacing;
            public float paddingLeft;
            public float paddingRight;
            public float paddingTop;
            public float paddingBottom;
            public FigmaRect absoluteBoundingBox;
            public FigmaStyle style;
            public FigmaPaint[] fills;
            public FigmaNode[] children;
        }
        [Serializable] private sealed class FigmaRect { public float x; public float y; public float width; public float height; }
        [Serializable] private sealed class FigmaStyle { public float fontSize; }
        [Serializable] private sealed class FigmaPaint { public string type; public bool visible = true; public float opacity = 1f; public FigmaColor color; }
        [Serializable] private sealed class FigmaColor { public float r; public float g; public float b; public float a = 1f; }

        /// <summary>Replaces <paramref name="target"/>'s elements with the first top-level Figma frame.</summary>
        /// <returns>The number of imported nodes.</returns>
        public static int ImportFirstFrame(string json, DesignerMetadataAsset target)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Figma JSON is empty.", nameof(json));
            if (target == null) throw new ArgumentNullException(nameof(target));
            var response = JsonUtility.FromJson<FileResponse>(json);
            var frame = FindFirst(response?.document, "FRAME") ?? FirstVisualChild(response?.document);
            if (frame == null || frame.absoluteBoundingBox == null)
                throw new InvalidOperationException("The Figma document contains no importable frame with bounds.");

            var imported = new List<DesignerElementMetadata>();
            var usedIds = new HashSet<string>(StringComparer.Ordinal);
            Convert(frame, null, frame.absoluteBoundingBox.x, frame.absoluteBoundingBox.y, imported, usedIds);
            target.elements.Clear();
            target.elements.AddRange(imported);
            target.schemaVersion = DesignerMetadataAsset.CurrentSchemaVersion;
            if (string.IsNullOrWhiteSpace(target.screenId)) target.screenId = SafeId(frame.name, "FigmaScreen");
            return imported.Count;
        }

        private static void Convert(FigmaNode node, string parentId, float originX, float originY,
            List<DesignerElementMetadata> output, HashSet<string> usedIds)
        {
            if (node?.absoluteBoundingBox == null) return;
            var id = UniqueId(SafeId(node.name, node.type ?? "Element"), usedIds);
            var element = new DesignerElementMetadata
            {
                elementId = id,
                displayName = string.IsNullOrWhiteSpace(node.name) ? id : node.name,
                parentId = parentId,
                siblingIndex = output.Count,
                elementType = TypeOf(node),
                rect = new Rect(node.absoluteBoundingBox.x - originX, node.absoluteBoundingBox.y - originY,
                    Mathf.Max(1f, node.absoluteBoundingBox.width), Mathf.Max(1f, node.absoluteBoundingBox.height)),
                text = node.characters ?? string.Empty,
                fontSize = node.style != null && node.style.fontSize > 0f ? Mathf.RoundToInt(node.style.fontSize) : 14,
                tint = FillColor(node, new Color(.15f, .22f, .34f, 1f))
            };
            if (element.elementType == "Label") element.textColor = element.tint;
            if (node.layoutMode == "HORIZONTAL" || node.layoutMode == "VERTICAL")
            {
                element.autoLayout.enabled = true;
                element.autoLayout.direction = node.layoutMode == "HORIZONTAL"
                    ? DesignerAutoLayoutDirection.Row : DesignerAutoLayoutDirection.Column;
                element.autoLayout.spacing = node.itemSpacing;
                element.autoLayout.paddingLeft = node.paddingLeft;
                element.autoLayout.paddingRight = node.paddingRight;
                element.autoLayout.paddingTop = node.paddingTop;
                element.autoLayout.paddingBottom = node.paddingBottom;
            }
            output.Add(element);

            if (node.children == null) return;
            for (var i = 0; i < node.children.Length; i++)
            {
                var before = output.Count;
                Convert(node.children[i], id, originX, originY, output, usedIds);
                if (output.Count > before) output[before].siblingIndex = i;
            }
        }

        private static string TypeOf(FigmaNode node)
        {
            switch (node.type)
            {
                case "TEXT": return "Label";
                case "VECTOR":
                case "ELLIPSE":
                case "LINE":
                case "BOOLEAN_OPERATION": return "Image";
                case "COMPONENT":
                case "INSTANCE": return "Card";
                case "GROUP": return "Container";
                default: return "Panel";
            }
        }

        private static Color FillColor(FigmaNode node, Color fallback)
        {
            if (node.fills == null) return fallback;
            foreach (var fill in node.fills)
                if (fill != null && fill.visible && fill.type == "SOLID" && fill.color != null)
                    return new Color(fill.color.r, fill.color.g, fill.color.b,
                        Mathf.Clamp01(fill.color.a * fill.opacity));
            return fallback;
        }

        private static FigmaNode FindFirst(FigmaNode node, string type)
        {
            if (node == null) return null;
            if (node.type == type && node.absoluteBoundingBox != null) return node;
            if (node.children == null) return null;
            foreach (var child in node.children)
            {
                var found = FindFirst(child, type);
                if (found != null) return found;
            }
            return null;
        }

        private static FigmaNode FirstVisualChild(FigmaNode node)
        {
            if (node?.children == null) return null;
            foreach (var child in node.children)
                if (child?.absoluteBoundingBox != null) return child;
            foreach (var child in node.children)
            {
                var found = FirstVisualChild(child);
                if (found != null) return found;
            }
            return null;
        }

        private static string SafeId(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value)) value = fallback;
            var chars = value.Trim().ToCharArray();
            for (var i = 0; i < chars.Length; i++)
                if (!char.IsLetterOrDigit(chars[i]) && chars[i] != '_') chars[i] = '_';
            var id = new string(chars).Trim('_');
            if (string.IsNullOrEmpty(id)) id = fallback;
            if (char.IsDigit(id[0])) id = "Element_" + id;
            return id;
        }

        private static string UniqueId(string baseId, HashSet<string> used)
        {
            var id = baseId;
            var suffix = 2;
            while (!used.Add(id)) id = baseId + "_" + suffix++;
            return id;
        }
    }
}
