using System;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// Linear fill direction for ProgressBar/StatBar previews, mirroring Unity's
    /// <c>Image.FillMethod.Horizontal</c>/<c>Vertical</c> + <c>fillOrigin</c> combination
    /// (LeftToRight/RightToLeft = Horizontal with Left/Right origin; BottomToTop/TopToBottom =
    /// Vertical with Bottom/Top origin).
    /// </summary>
    public enum DesignerFillDirection
    {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom
    }

    /// <summary>
    /// Unity-Image-like fill configuration for value-driven Designer canvas previews
    /// (ProgressBar, StatBar, RadialFill, Spinner). <see cref="DesignerElementMetadata.previewValue"/>
    /// is normalized against <see cref="minValue"/>/<see cref="maxValue"/> before being applied,
    /// same as Unity's Slider min/max rather than assuming a fixed 0-100 range.
    /// </summary>
    [Serializable]
    public sealed class DesignerFillMetadata
    {
        public float minValue;
        public float maxValue = 100f;
        public DesignerFillDirection direction = DesignerFillDirection.LeftToRight;
        /// <summary>RadialFill/Spinner only - sweep direction of the arc, matching Unity's Image.fillClockwise.</summary>
        public bool clockwise = true;
    }
}
