using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    /// <summary>
    /// Custom-painted radial progress ring for the Designer canvas preview of RadialFill/Spinner
    /// elements, using UI Toolkit's <see cref="Painter2D"/> arc drawing (no texture/sprite
    /// needed). Draws a full background ring, then a foreground arc from the top (-90deg)
    /// sweeping clockwise by <see cref="Value"/> percent of a full turn.
    /// </summary>
    public sealed class RadialFillPreview : VisualElement
    {
        private float _value = 60f;
        private Color _trackColor = new Color(1f, 1f, 1f, 0.12f);
        private Color _fillColor = new Color(0.26f, 0.83f, 0.76f, 1f);
        private float _thickness = 6f;
        private bool _spin;
        private bool _clockwise = true;

        /// <summary>0-100. Ignored (full spin ring instead) when <see cref="Spin"/> is true.</summary>
        public float Value
        {
            get => _value;
            set { _value = Mathf.Clamp(value, 0f, 100f); MarkDirtyRepaint(); }
        }

        public Color TrackColor
        {
            get => _trackColor;
            set { _trackColor = value; MarkDirtyRepaint(); }
        }

        public Color FillColor
        {
            get => _fillColor;
            set { _fillColor = value; MarkDirtyRepaint(); }
        }

        public float Thickness
        {
            get => _thickness;
            set { _thickness = value; MarkDirtyRepaint(); }
        }

        /// <summary>Sweep direction, matching Unity's Image.fillClockwise.</summary>
        public bool Clockwise
        {
            get => _clockwise;
            set { _clockwise = value; MarkDirtyRepaint(); }
        }

        /// <summary>Spinner mode: draws a fixed-length arc that rotates continuously instead of a value-driven fill.</summary>
        public bool Spin
        {
            get => _spin;
            set
            {
                _spin = value;
                if (value) schedule.Execute(Rotate).Every(30);
            }
        }

        private float _spinAngle;

        public RadialFillPreview()
        {
            generateVisualContent += OnGenerateVisualContent;
        }

        private void Rotate()
        {
            _spinAngle = (_spinAngle + 12f) % 360f;
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var rect = contentRect;
            if (rect.width <= 0f || rect.height <= 0f) return;

            var center = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
            var radius = Mathf.Max(1f, Mathf.Min(rect.width, rect.height) * 0.5f - _thickness * 0.5f);

            var painter = ctx.painter2D;
            painter.lineWidth = _thickness;
            painter.lineCap = LineCap.Round;

            // Background track: full circle.
            painter.strokeColor = _trackColor;
            painter.BeginPath();
            painter.Arc(center, radius, 0f, 360f);
            painter.Stroke();

            // Foreground fill/spin arc, starting at the top (-90deg) sweeping clockwise or
            // counter-clockwise depending on Clockwise (Unity's Image.fillClockwise).
            var sign = _clockwise ? 1f : -1f;
            painter.strokeColor = _fillColor;
            painter.BeginPath();
            if (_spin)
            {
                var start = _spinAngle * sign - 90f;
                painter.Arc(center, radius, start, start + 90f * sign);
            }
            else if (_value > 0f)
            {
                var sweep = 360f * (_value / 100f) * sign;
                painter.Arc(center, radius, -90f, -90f + sweep);
            }
            painter.Stroke();
        }
    }
}
