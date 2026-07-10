using System;

namespace emiteat.NexUI.Designer
{
    /// <summary>C2: main-axis direction of an Auto Layout container, mirroring Figma's Horizontal/Vertical.</summary>
    public enum DesignerAutoLayoutDirection
    {
        Row,
        Column
    }

    /// <summary>
    /// C2: how an element sizes itself along one axis when it is a child of an Auto Layout
    /// container - Figma's Hug/Fill/Fixed model. Hug = size to content, Fill = grow to take
    /// remaining space along that axis, Fixed = use this element's own <c>rect</c> size.
    /// </summary>
    public enum DesignerAutoLayoutSizing
    {
        Fixed,
        Hug,
        Fill
    }

    /// <summary>
    /// C2: Auto Layout settings for one element. <see cref="Enabled"/> turns the element into
    /// an Auto Layout *container* that arranges its direct children (by <see cref="parentId"/>)
    /// along <see cref="direction"/> with <see cref="spacing"/> between them and
    /// <see cref="paddingLeft"/>/<see cref="paddingTop"/>/<see cref="paddingRight"/>/
    /// <see cref="paddingBottom"/> inset from its own rect. <see cref="widthSizing"/>/
    /// <see cref="heightSizing"/> describe how *this* element sizes itself when it is a child
    /// of an Auto Layout container (irrelevant otherwise). Maps directly onto UI Toolkit's USS
    /// flex properties (flex-direction/padding/flex-grow) - no new layout engine is introduced.
    /// </summary>
    [Serializable]
    public sealed class DesignerAutoLayoutMetadata
    {
        public bool enabled;
        public DesignerAutoLayoutDirection direction = DesignerAutoLayoutDirection.Column;
        public float spacing;
        public float paddingLeft;
        public float paddingTop;
        public float paddingRight;
        public float paddingBottom;
        public DesignerAutoLayoutSizing widthSizing = DesignerAutoLayoutSizing.Fixed;
        public DesignerAutoLayoutSizing heightSizing = DesignerAutoLayoutSizing.Fixed;
    }
}
