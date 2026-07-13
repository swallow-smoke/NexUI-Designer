using emiteat.NexUI.Designer.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    /// <summary>
    /// Draws every authored <c>DesignerFocusMetadata</c> directional link on the canvas as a line
    /// between element centers, plus a marker on whichever element is set as the screen's default
    /// focus. Read-only - editing links happens in <c>FocusNavigationPanel</c>, not by dragging
    /// lines here (a drag-to-connect canvas UI is future polish, not required for the links to work).
    /// </summary>
    public sealed class FocusNavigationOverlay : VisualElement
    {
        private static readonly Color LinkColor = new Color(0.98f, 0.75f, 0.18f, 0.8f);
        private static readonly Color DefaultFocusColor = new Color(0.98f, 0.75f, 0.18f, 1f);

        private readonly NexUIDesignerContext _context;
        private readonly ContextBoundSubscriptions _subscriptions;

        public FocusNavigationOverlay(NexUIDesignerContext context)
        {
            _context = context;
            name = "FocusNavigationOverlay";
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;
            generateVisualContent += OnGenerateVisualContent;

            _subscriptions = new ContextBoundSubscriptions(this);
            _subscriptions.Add<DesignerMetadataAsset>(h => context.MetadataChanged += h, h => context.MetadataChanged -= h, _ => MarkDirtyRepaint());
            _subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, MarkDirtyRepaint);
            _subscriptions.Add(h => context.PreviewSettingsChanged += h, h => context.PreviewSettingsChanged -= h, MarkDirtyRepaint);
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            if (!_context.ShowFocusNav || _context.Metadata == null) return;

            var zoom = _context.Zoom;
            var painter = ctx.painter2D;
            painter.strokeColor = LinkColor;
            painter.lineWidth = 1.5f;

            foreach (var element in _context.Metadata.elements)
            {
                if (element == null) continue;
                var from = element.rect.center * zoom;

                DrawLinkTo(painter, from, element.focus.upElementId, zoom);
                DrawLinkTo(painter, from, element.focus.downElementId, zoom);
                DrawLinkTo(painter, from, element.focus.leftElementId, zoom);
                DrawLinkTo(painter, from, element.focus.rightElementId, zoom);

                if (element.focus.isDefaultFocus)
                {
                    painter.BeginPath();
                    painter.Arc(from, 6f, Angle.Degrees(0f), Angle.Degrees(360f));
                    painter.fillColor = DefaultFocusColor;
                    painter.Fill();
                }
            }
        }

        private void DrawLinkTo(Painter2D painter, Vector2 from, string targetElementId, float zoom)
        {
            if (string.IsNullOrEmpty(targetElementId)) return;
            var target = _context.Metadata.Find(targetElementId);
            if (target == null) return;

            var to = target.rect.center * zoom;
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.Stroke();
        }
    }
}
