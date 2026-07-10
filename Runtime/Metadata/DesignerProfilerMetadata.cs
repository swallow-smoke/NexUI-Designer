using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// A point-in-time measurement of NexUI runtime cost. Collected by the profiler
    /// service from the live UIManager and displayed / exported by the profiler panel.
    /// </summary>
    [Serializable]
    public sealed class UIProfilerSnapshot
    {
        public int openedScreenCount;
        public int elementCount;
        public int bindingCount;
        public int activeMotionCount;

        public float lastScreenOpenMs;
        public float lastScreenCloseMs;
        public float lastThemeApplyMs;
        public float lastBindingUpdateMs;

        // ---- B4 static heuristic estimates (design-time, no live GPU/Profiler data) ---------
        // These are approximations from Designer metadata (element type + rect + tint), not
        // measurements from a running Profiler/Frame Debugger session - use them to catch
        // obviously-expensive screens early, not as exact numbers in a performance claim.

        /// <summary>Rough vertex count from per-element-type quad/9-slice/glyph estimates.</summary>
        public int estimatedVertexCount;

        /// <summary>Suggested UI Toolkit Panel Settings "Vertex Budget" for this screen (estimatedVertexCount with headroom).</summary>
        public int recommendedVertexBudget;

        /// <summary>
        /// Distinct-tint-group count, used as a texture/material batching proxy in the absence of
        /// a real sprite/texture reference on element metadata. Compare against the uGUI
        /// 8-texture-per-batch guideline; treat as an approximation, not an exact draw-call count.
        /// </summary>
        public int estimatedBatchGroups;

        /// <summary>Sum of pairwise element rect overlap area, divided by canvas area - a rough overdraw multiplier (0 = no overlap).</summary>
        public float overdrawScore;

        /// <summary>Count of motion-bound elements that aren't the topmost (last) sibling among same-parent elements - each one risks a full-hierarchy rebatch when it animates.</summary>
        public int motionOrderWarnings;
    }

    /// <summary>A recorded series of profiler snapshots for a session.</summary>
    [Serializable]
    public sealed class DesignerProfilerMetadata
    {
        public List<UIProfilerSnapshot> records = new();
    }
}
