using emiteat.NexUI.Abstractions;
using emiteat.NexUI.MotionClip;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Draws a small 0..1 curve thumbnail for a <see cref="UIMotionEasing"/> using UI Toolkit's
    /// native <see cref="Painter2D"/> (no baked textures, no IMGUI) - sampled from the exact same
    /// <see cref="UIMotionClipEvaluator.Ease"/> function that plays at runtime, so the preview can
    /// never drift from real playback.
    /// </summary>
    public sealed class EasingCurvePreview : VisualElement
    {
        private const int SampleCount = 24;
        private UIMotionEasing _easing;

        public EasingCurvePreview(UIMotionEasing easing, float width = 48f, float height = 28f)
        {
            _easing = easing;
            style.width = width;
            style.height = height;
            generateVisualContent += OnGenerateVisualContent;
        }

        public void SetEasing(UIMotionEasing easing)
        {
            if (_easing == easing) return;
            _easing = easing;
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var rect = contentRect;
            if (rect.width <= 0f || rect.height <= 0f) return;

            var painter = context.painter2D;
            painter.strokeColor = new Color(0.66f, 0.63f, 1f, 0.9f);
            painter.lineWidth = 1.5f;
            painter.BeginPath();

            for (var i = 0; i <= SampleCount; i++)
            {
                var t = i / (float)SampleCount;
                var eased = UIMotionClipEvaluator.Ease(_easing, t);
                var x = t * rect.width;
                var y = rect.height - Mathf.Clamp01(eased) * rect.height;
                if (i == 0) painter.MoveTo(new Vector2(x, y));
                else painter.LineTo(new Vector2(x, y));
            }

            painter.Stroke();
        }
    }
}
