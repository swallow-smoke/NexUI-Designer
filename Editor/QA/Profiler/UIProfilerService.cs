using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Profiler
{
    /// <summary>
    /// Builds a <see cref="UIProfilerSnapshot"/> from Designer metadata (static counts plus B4
    /// heuristic cost estimates). Live timings (screen open/close/theme-apply ms) are only
    /// populated when captured from a running UIManager; this design-time capture fills the
    /// structural counts and estimates.
    /// </summary>
    public static class UIProfilerService
    {
        /// <summary>Assumed canvas size for the overdraw-fraction estimate when no live screen resolution is known (matches the Designer's own default canvas resolution).</summary>
        private static readonly Vector2 AssumedCanvasSize = new Vector2(1920, 1080);

        private static readonly Dictionary<string, int> VertexCostByType = new Dictionary<string, int>
        {
            ["Panel"] = 9, ["Card"] = 9, ["Modal"] = 9, ["Container"] = 9, ["Toast"] = 9,
            ["Tooltip"] = 9, ["Popover"] = 9, ["Skeleton"] = 9, ["ChoiceList"] = 9, ["Slot"] = 9,
            ["Image"] = 4, ["IconButton"] = 8,
            ["ProgressBar"] = 18, ["StatBar"] = 18, ["RadialFill"] = 24, ["Spinner"] = 24,
            ["List"] = 4, ["Grid"] = 4,
        };

        public static UIProfilerSnapshot CaptureStatic(DesignerMetadataAsset asset)
        {
            var s = new UIProfilerSnapshot { openedScreenCount = asset != null ? 1 : 0 };
            if (asset == null) return s;

            s.elementCount = asset.elements.Count;

            int bindings = 0, motions = 0, vertices = 0;
            var batchGroups = new HashSet<(bool isText, int r, int g, int b, int a)>();
            var byParent = new Dictionary<string, List<int>>(); // parentId -> indices, in declared (z) order

            for (int i = 0; i < asset.elements.Count; i++)
            {
                var e = asset.elements[i];
                if (e == null) continue;

                if (e.binding != null)
                {
                    if (!string.IsNullOrEmpty(e.binding.textKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.valueKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.visibilityKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.interactableKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.classKey)) bindings++;
                    if (!string.IsNullOrEmpty(e.binding.commandKey)) bindings++;
                }
                var hasMotion = e.motion != null && !string.IsNullOrEmpty(e.motion.motionId);
                if (hasMotion) motions++;

                vertices += EstimateVertexCost(e);

                var isText = e.elementType == "Label" || e.elementType == "Button";
                batchGroups.Add((isText, Quantize(e.tint.r), Quantize(e.tint.g), Quantize(e.tint.b), Quantize(e.tint.a)));

                var parentKey = e.parentId ?? string.Empty;
                if (!byParent.TryGetValue(parentKey, out var list))
                    byParent[parentKey] = list = new List<int>();
                list.Add(i);
            }

            s.bindingCount = bindings;
            s.activeMotionCount = motions;
            s.estimatedVertexCount = vertices;
            s.recommendedVertexBudget = Mathf.CeilToInt(vertices * 1.5f / 100f) * 100; // round up to a clean headroom figure
            s.estimatedBatchGroups = batchGroups.Count;
            s.overdrawScore = EstimateOverdraw(asset.elements);
            s.motionOrderWarnings = CountMotionOrderWarnings(asset.elements, byParent);

            return s;
        }

        private static int EstimateVertexCost(DesignerElementMetadata e)
        {
            if (VertexCostByType.TryGetValue(e.elementType, out var baseCost))
            {
                if (e.elementType == "IconButton") return baseCost + TextVertexCost(e.text);
                return baseCost;
            }
            if (e.elementType == "Button") return 4 + TextVertexCost(string.IsNullOrEmpty(e.text) ? "Button" : e.text);
            if (e.elementType == "Label") return TextVertexCost(e.text);
            return 4; // default: a plain quad
        }

        private static int TextVertexCost(string text)
        {
            var length = string.IsNullOrEmpty(text) ? 8 : text.Length; // assume ~8 glyphs when a bound/empty label's runtime text isn't known
            return length * 4;
        }

        private static int Quantize(float channel) => Mathf.RoundToInt(Mathf.Clamp01(channel) * 20f); // 0..20 buckets (~5% steps)

        private static float EstimateOverdraw(List<DesignerElementMetadata> elements)
        {
            var canvasArea = AssumedCanvasSize.x * AssumedCanvasSize.y;
            if (canvasArea <= 0f) return 0f;

            float overlapArea = 0f;
            for (int i = 0; i < elements.Count; i++)
            {
                var a = elements[i];
                if (a == null || a.hiddenInDesigner) continue;
                for (int j = i + 1; j < elements.Count; j++)
                {
                    var b = elements[j];
                    if (b == null || b.hiddenInDesigner) continue;
                    var intersection = RectIntersectionArea(a.rect, b.rect);
                    if (intersection > 0f) overlapArea += intersection;
                }
            }
            return overlapArea / canvasArea;
        }

        private static float RectIntersectionArea(Rect a, Rect b)
        {
            var xMin = Mathf.Max(a.xMin, b.xMin);
            var xMax = Mathf.Min(a.xMax, b.xMax);
            var yMin = Mathf.Max(a.yMin, b.yMin);
            var yMax = Mathf.Min(a.yMax, b.yMax);
            var w = xMax - xMin;
            var h = yMax - yMin;
            return (w > 0f && h > 0f) ? w * h : 0f;
        }

        private static int CountMotionOrderWarnings(List<DesignerElementMetadata> elements, Dictionary<string, List<int>> byParent)
        {
            var warnings = 0;
            foreach (var siblings in byParent.Values)
            {
                if (siblings.Count < 2) continue;
                var topmostIndex = siblings[siblings.Count - 1];
                foreach (var index in siblings)
                {
                    if (index == topmostIndex) continue;
                    var e = elements[index];
                    if (e?.motion != null && !string.IsNullOrEmpty(e.motion.motionId))
                        warnings++;
                }
            }
            return warnings;
        }

        public static string ToJson(UIProfilerSnapshot snapshot) => JsonUtility.ToJson(snapshot, true);

        public static List<string> Warnings(UIProfilerSnapshot s)
        {
            var messages = new List<string>();
            if (s.elementCount > 200) messages.Add($"• high element count: {s.elementCount}");
            if (s.bindingCount > 150) messages.Add($"• high binding count: {s.bindingCount}");
            if (s.activeMotionCount > 32) messages.Add($"• high active motion count: {s.activeMotionCount}");
            if (s.estimatedBatchGroups > 8)
                messages.Add($"• ~{s.estimatedBatchGroups} distinct tint/text groups (proxy for texture/material batches) exceed the 8-per-batch uGUI guideline - consider atlasing or reusing tint values.");
            if (s.overdrawScore > 0.5f)
                messages.Add($"• overlap estimate {s.overdrawScore:P0} of canvas area - check for redundant background panels stacked under visible content.");
            if (s.motionOrderWarnings > 0)
                messages.Add($"• {s.motionOrderWarnings} animated element(s) aren't the topmost sibling - moving them last in the hierarchy avoids rebatching static siblings above them.");
            return messages;
        }

        /// <summary>Names the single largest contributor to this snapshot's estimated cost, for a one-line "why is this screen expensive" readout.</summary>
        public static string ClassifyBottleneck(UIProfilerSnapshot s)
        {
            var scores = new (string label, float score)[]
            {
                ("element count", s.elementCount / 200f),
                ("overdraw", s.overdrawScore / 0.5f),
                ("motion count", s.activeMotionCount / 32f),
                ("draw-call batching", s.estimatedBatchGroups / 8f),
                ("binding count", s.bindingCount / 150f),
            };

            var best = ("none", 0f);
            foreach (var candidate in scores)
                if (candidate.score > best.Item2) best = candidate;

            return best.Item2 >= 1f ? best.Item1 : "none (within guideline thresholds)";
        }
    }
}
