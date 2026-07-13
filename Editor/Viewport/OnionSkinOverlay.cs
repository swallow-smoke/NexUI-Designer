using System.Linq;
using emiteat.NexUI.Designer.Editor;
using emiteat.NexUI.MotionClip;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    /// <summary>
    /// Ghosts the selected element's rect at the nearest keyframe times immediately before/after
    /// the current Motion Clip Editor scrub position (across every property track on that element,
    /// not just AnchoredPosition), so its motion can be judged without repeatedly scrubbing back and
    /// forth. Read-only overlay - draws nothing if Onion Skin is off, no clip is open, nothing is
    /// selected, or the selection has no tracks in the open clip.
    /// </summary>
    public sealed class OnionSkinOverlay : VisualElement
    {
        private static readonly Color GhostColor = new Color(1f, 1f, 1f, 0.35f);

        private readonly NexUIDesignerContext _context;
        private readonly ContextBoundSubscriptions _subscriptions;

        public OnionSkinOverlay(NexUIDesignerContext context)
        {
            _context = context;
            name = "OnionSkinOverlay";
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;
            generateVisualContent += OnGenerateVisualContent;

            _subscriptions = new ContextBoundSubscriptions(this);
            _subscriptions.Add(h => context.ActiveMotionClipChanged += h, h => context.ActiveMotionClipChanged -= h, MarkDirtyRepaint);
            _subscriptions.Add<DesignerElementMetadata>(h => context.MetadataSelectionChanged += h, h => context.MetadataSelectionChanged -= h, _ => MarkDirtyRepaint());
            _subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, MarkDirtyRepaint);
            _subscriptions.Add(h => context.PreviewSettingsChanged += h, h => context.PreviewSettingsChanged -= h, MarkDirtyRepaint);
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var element = _context.SelectedMetadata;
            if (!_context.ShowOnionSkin || _context.ActiveMotionClip == null || element == null) return;

            var track = _context.ActiveMotionClip.tracks?.FirstOrDefault(t => t.targetElementId == element.elementId);
            if (track?.propertyTracks == null || track.propertyTracks.Length == 0) return;

            var current = _context.ActiveMotionClipTime;
            float? previous = null;
            float? next = null;
            foreach (var propertyTrack in track.propertyTracks)
            {
                if (propertyTrack.keyframes == null) continue;
                foreach (var keyframe in propertyTrack.keyframes)
                {
                    var time = keyframe.time;
                    if (time < current - 0.0001f && (!previous.HasValue || time > previous.Value))
                        previous = time;
                    else if (time > current + 0.0001f && (!next.HasValue || time < next.Value))
                        next = time;
                }
            }

            var painter = ctx.painter2D;
            if (previous.HasValue) DrawGhost(painter, track, element, previous.Value);
            if (next.HasValue) DrawGhost(painter, track, element, next.Value);
        }

        private void DrawGhost(Painter2D painter, UIMotionClipTrack track, DesignerElementMetadata element, float time)
        {
            var position = element.rect.position;
            var size = element.rect.size;

            foreach (var propertyTrack in track.propertyTracks)
            {
                var value = UIMotionClipEvaluator.Evaluate(propertyTrack, time);
                if (!value.HasValue) continue;
                if (propertyTrack.propertyType == UIMotionClipPropertyType.AnchoredPosition) position = value.Value.vector2Value;
                else if (propertyTrack.propertyType == UIMotionClipPropertyType.SizeDelta) size = value.Value.vector2Value;
            }

            var zoom = _context.Zoom;
            var rect = new Rect(position * zoom, size * zoom);

            painter.strokeColor = GhostColor;
            painter.lineWidth = 1f;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMin));
            painter.Stroke();
        }
    }
}
