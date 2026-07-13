using System.Linq;
using emiteat.NexUI.MotionClip;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    /// <summary>
    /// Draws the AnchoredPosition track (if any) of whichever <see cref="NexUIDesignerContext.ActiveMotionClip"/>
    /// is open in the Motion Clip Editor for the currently selected element: a connected path through
    /// every keyframe, a dot per keyframe, and a marker at the current scrub time. Read-only overlay -
    /// never mutates metadata or the clip, and draws nothing if Motion Path is off, no clip is open,
    /// nothing is selected, or the selection has no AnchoredPosition track.
    /// </summary>
    public sealed class MotionPathOverlay : VisualElement
    {
        private static readonly Color LineColor = new Color(0.26f, 0.9f, 0.76f, 0.85f);
        private static readonly Color KeyframeColor = new Color(0.26f, 0.9f, 0.76f, 0.95f);
        private static readonly Color CurrentColor = new Color(1f, 1f, 1f, 0.95f);

        private readonly NexUIDesignerContext _context;

        public MotionPathOverlay(NexUIDesignerContext context)
        {
            _context = context;
            name = "MotionPathOverlay";
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;
            generateVisualContent += OnGenerateVisualContent;

            context.ActiveMotionClipChanged += MarkDirtyRepaint;
            context.MetadataSelectionChanged += _ => MarkDirtyRepaint();
            context.CanvasChanged += MarkDirtyRepaint;
            context.PreviewSettingsChanged += MarkDirtyRepaint;
        }

        private UIMotionClipPropertyTrack FindPositionTrack(
            out DesignerElementMetadata element)
        {
            element = _context.SelectedMetadata;

            if (!_context.ShowMotionPath ||
                _context.ActiveMotionClip == null ||
                element == null)
            {
                return null;
            }

            var elementId = element.elementId;

            var track = _context.ActiveMotionClip.tracks?
                .FirstOrDefault(track => track.targetElementId == elementId);

            return track?.propertyTracks?
                .FirstOrDefault(propertyTrack =>
                    propertyTrack.propertyType ==
                    UIMotionClipPropertyType.AnchoredPosition);
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var propertyTrack = FindPositionTrack(out var element);
            if (propertyTrack?.keyframes == null || propertyTrack.keyframes.Length < 2) return;

            var zoom = _context.Zoom;
            var halfSize = element.rect.size * 0.5f;
            var painter = ctx.painter2D;

            painter.strokeColor = LineColor;
            painter.lineWidth = 1.5f;
            painter.BeginPath();
            for (var i = 0; i < propertyTrack.keyframes.Length; i++)
            {
                var point = (propertyTrack.keyframes[i].value.vector2Value + halfSize) * zoom;
                if (i == 0) painter.MoveTo(point);
                else painter.LineTo(point);
            }
            painter.Stroke();

            for (var i = 0; i < propertyTrack.keyframes.Length; i++)
            {
                var point = (propertyTrack.keyframes[i].value.vector2Value + halfSize) * zoom;
                painter.BeginPath();
                painter.Arc(point, 3f, Angle.Degrees(0f), Angle.Degrees(360f));
                painter.fillColor = KeyframeColor;
                painter.Fill();
            }

            var currentValue = UIMotionClipEvaluator.Evaluate(propertyTrack, _context.ActiveMotionClipTime);
            if (currentValue.HasValue)
            {
                var point = (currentValue.Value.vector2Value + halfSize) * zoom;
                painter.BeginPath();
                painter.Arc(point, 5f, Angle.Degrees(0f), Angle.Degrees(360f));
                painter.fillColor = CurrentColor;
                painter.Fill();
            }
        }
    }
}
