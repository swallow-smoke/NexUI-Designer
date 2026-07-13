using System;
using System.Collections.Generic;
using emiteat.NexUI.MotionClip;
using emiteat.NexUI.MotionGraph;

namespace emiteat.NexUI.Designer
{
    /// <summary>Events that can start a clip authored for a screen or element.</summary>
    public enum DesignerMotionTrigger
    {
        ScreenEnter,
        ScreenExit,
        HoverEnter,
        HoverExit,
        PointerDown,
        PointerUp,
        Click,
        Selected,
        Deselected,
        Enabled,
        Disabled,
        StateEnter,
        StateExit,
        CommandStarted,
        CommandCompleted,
        CommandFailed
    }

    /// <summary>A persistent reference from a Designer trigger to an independent Motion Clip asset.</summary>
    [Serializable]
    public sealed class DesignerMotionBinding
    {
        public string bindingId;
        public string targetElementId;
        public DesignerMotionTrigger trigger;
        public string stateId;
        public string commandId;
        public UIMotionClip clip;
        public UIMotionClip reducedMotionClip;
    }

    /// <summary>
    /// Screen-owned motion configuration. Clips remain independent assets and are referenced,
    /// never copied into Designer metadata.
    /// </summary>
    [Serializable]
    public sealed class DesignerScreenMotionMetadata
    {
        public UIMotionClip entryClip;
        public UIMotionClip exitClip;
        public List<DesignerMotionBinding> bindings = new List<DesignerMotionBinding>();
        public UIMotionStateMachine stateMachine;
        public UIMotionGraphAsset motionGraph;
    }
}
