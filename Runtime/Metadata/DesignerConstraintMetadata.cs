using System;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// C2: pin behavior for one axis when its parent is resized (e.g. a breakpoint change in
    /// C1's responsive preview) - Figma's constraints model. Start = pin to left/top edge,
    /// End = pin to right/bottom edge, Center = keep centered, Scale = resize proportionally
    /// with the parent.
    /// </summary>
    public enum DesignerConstraintMode
    {
        Start,
        End,
        Center,
        Scale
    }

    /// <summary>C2: per-axis parent-resize pin behavior for one element.</summary>
    [Serializable]
    public sealed class DesignerConstraintMetadata
    {
        public DesignerConstraintMode horizontal = DesignerConstraintMode.Start;
        public DesignerConstraintMode vertical = DesignerConstraintMode.Start;
    }
}
