using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Productivity
{
    public enum DesignerDetectedLayout { Vertical, Horizontal, Grid }

    [Serializable]
    public sealed class DesignerLayoutAnalysis
    {
        public DesignerDetectedLayout Layout;
        public float Spacing;
        public float PaddingLeft, PaddingRight, PaddingTop, PaddingBottom;
        public bool SameSize;
        public string Alignment;
        public string Warning;
    }

    /// <summary>Pure layout inference plus Undo-aware application through the existing Designer context.</summary>
    public static class DesignerLayoutAnalysisService
    {
        public static DesignerLayoutAnalysis Analyze(IReadOnlyList<DesignerElementMetadata> selection)
        {
            var items = selection?.Where(x => x != null).ToList() ?? new List<DesignerElementMetadata>();
            if (items.Count < 2) return new DesignerLayoutAnalysis { Warning = "두 개 이상의 Element를 선택하세요." };
            var xSpread = items.Max(x => x.rect.center.x) - items.Min(x => x.rect.center.x);
            var ySpread = items.Max(x => x.rect.center.y) - items.Min(x => x.rect.center.y);
            var rows = Cluster(items.Select(x => x.rect.center.y));
            var cols = Cluster(items.Select(x => x.rect.center.x));
            var layout = rows > 1 && cols > 1 ? DesignerDetectedLayout.Grid : ySpread >= xSpread ? DesignerDetectedLayout.Vertical : DesignerDetectedLayout.Horizontal;
            var ordered = layout == DesignerDetectedLayout.Horizontal ? items.OrderBy(x => x.rect.x).ToList() : items.OrderBy(x => x.rect.y).ToList();
            var gaps = new List<float>();
            for (var i = 1; i < ordered.Count; i++) gaps.Add(layout == DesignerDetectedLayout.Horizontal ? ordered[i].rect.xMin - ordered[i - 1].rect.xMax : ordered[i].rect.yMin - ordered[i - 1].rect.yMax);
            var bounds = Bounds(items);
            return new DesignerLayoutAnalysis
            {
                Layout = layout,
                Spacing = gaps.Count == 0 ? 0 : Mathf.Max(0, gaps.OrderBy(x => x).ElementAt(gaps.Count / 2)),
                PaddingLeft = items.Min(x => x.rect.xMin) - bounds.xMin,
                PaddingRight = bounds.xMax - items.Max(x => x.rect.xMax),
                PaddingTop = items.Min(x => x.rect.yMin) - bounds.yMin,
                PaddingBottom = bounds.yMax - items.Max(x => x.rect.yMax),
                SameSize = items.All(x => Mathf.Abs(x.rect.width - items[0].rect.width) < 1f && Mathf.Abs(x.rect.height - items[0].rect.height) < 1f),
                Alignment = DetectAlignment(items, layout),
                Warning = null
            };
        }

        public static DesignerElementMetadata Apply(NexUIDesignerContext context, DesignerLayoutAnalysis analysis)
        {
            if (context == null || analysis == null || context.SelectedElements.Count < 2) return null;
            var members = context.SelectedElements.ToList();
            DesignerElementMetadata container = null;
            NexUIDesignerUndo.Group("Convert Selection to Auto Layout", () =>
            {
                context.SelectMany(members);
                container = context.GroupSelection();
                if (container == null) return;
                context.UpdateElement(container, e =>
                {
                    e.displayName = "Auto Layout";
                    e.autoLayout.enabled = true;
                    e.autoLayout.direction = analysis.Layout == DesignerDetectedLayout.Vertical ? DesignerAutoLayoutDirection.Column
                        : analysis.Layout == DesignerDetectedLayout.Grid ? DesignerAutoLayoutDirection.Grid : DesignerAutoLayoutDirection.Row;
                    e.autoLayout.spacing = analysis.Spacing;
                    if (analysis.Layout == DesignerDetectedLayout.Grid)
                    {
                        var children = members.Where(x => x != null).ToList();
                        e.autoLayout.gridColumns = Mathf.Max(1, Cluster(children.Select(x => x.rect.center.x)));
                        e.autoLayout.gridCellWidth = children.Count > 0 ? children.Average(x => x.rect.width) : 100f;
                        e.autoLayout.gridCellHeight = children.Count > 0 ? children.Average(x => x.rect.height) : 100f;
                    }
                    e.autoLayout.paddingLeft = analysis.PaddingLeft; e.autoLayout.paddingRight = analysis.PaddingRight;
                    e.autoLayout.paddingTop = analysis.PaddingTop; e.autoLayout.paddingBottom = analysis.PaddingBottom;
                }, "Convert Selection to Auto Layout");
            });
            return container;
        }

        private static int Cluster(IEnumerable<float> values)
        {
            var sorted = values.OrderBy(x => x).ToList(); if (sorted.Count == 0) return 0;
            var count = 1; for (var i = 1; i < sorted.Count; i++) if (Mathf.Abs(sorted[i] - sorted[i - 1]) > 8f) count++;
            return count;
        }

        private static string DetectAlignment(IReadOnlyList<DesignerElementMetadata> items, DesignerDetectedLayout layout)
        {
            bool Close(IEnumerable<float> values) => values.Max() - values.Min() < 3f;
            if (layout == DesignerDetectedLayout.Vertical)
            {
                if (Close(items.Select(x => x.rect.xMin))) return "Left";
                if (Close(items.Select(x => x.rect.center.x))) return "Center";
                if (Close(items.Select(x => x.rect.xMax))) return "Right";
            }
            else if (layout == DesignerDetectedLayout.Horizontal)
            {
                if (Close(items.Select(x => x.rect.yMin))) return "Top";
                if (Close(items.Select(x => x.rect.center.y))) return "Middle";
                if (Close(items.Select(x => x.rect.yMax))) return "Bottom";
            }
            return "Mixed";
        }

        private static Rect Bounds(IReadOnlyList<DesignerElementMetadata> list)
        {
            var minX = list.Min(x => x.rect.xMin); var minY = list.Min(x => x.rect.yMin); var maxX = list.Max(x => x.rect.xMax); var maxY = list.Max(x => x.rect.yMax);
            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
    }

    public static class DesignerAnchorRecommendationService
    {
        public static DesignerAnchorPreset Recommend(Rect rect, Vector2 referenceSize)
        {
            if (referenceSize.x <= 0 || referenceSize.y <= 0) return DesignerAnchorPreset.TopLeft;
            if (rect.width >= referenceSize.x * .85f && rect.height >= referenceSize.y * .85f) return DesignerAnchorPreset.Stretch;
            var nx = rect.center.x / referenceSize.x; var ny = rect.center.y / referenceSize.y;
            var x = nx < .33f ? 0 : nx > .67f ? 2 : 1; var y = ny < .33f ? 0 : ny > .67f ? 2 : 1;
            var map = new[,] { { DesignerAnchorPreset.TopLeft, DesignerAnchorPreset.Top, DesignerAnchorPreset.TopRight }, { DesignerAnchorPreset.Left, DesignerAnchorPreset.Center, DesignerAnchorPreset.Right }, { DesignerAnchorPreset.BottomLeft, DesignerAnchorPreset.Bottom, DesignerAnchorPreset.BottomRight } };
            return map[y, x];
        }

        public static void Apply(NexUIDesignerContext context, Vector2 referenceSize)
        {
            if (context == null) return;
            var rects = context.SelectedElements.ToDictionary(x => x, x => x.rect);
            NexUIDesignerUndo.Group("Apply Recommended Anchors", () =>
            {
                foreach (var pair in rects) context.UpdateElement(pair.Key, e => { e.anchorPreset = Recommend(pair.Value, referenceSize); e.rect = pair.Value; }, "Apply Recommended Anchor");
            });
        }
    }
}
